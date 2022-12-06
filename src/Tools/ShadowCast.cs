/*
 * Copyright 2016 faddenSoft. All Rights Reserved.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using Godot;
using SatiRogue.Ecs.MapGenerator.Components;

/*
Field-of-vision calculation for a simple tiled map.

Based on http://www.roguebasin.com/index.php?title=FOV_using_recursive_shadowcasting , but
without the strange ideas about coordinate systems.

Computation is performed in the first octant, i.e. the triangle with vertices
{ (0,0), (N,0), (N,N) } on a Cartesian plane.  The cell grid identifies the center of 1x1
cells, so a cell at (X,Y) extends 0.5 in each direction.  The viewer is at (0,0), and we
assume that the viewer's cell is visible to itself.

Cells are assumed to be in shadow unless light can reach them, so the caller should
reset all cells to "not visible" before calling this.


External dependencies:
- Access to the map is provided by an "ICellGrid" implementation, which has two methods:
    bool IsWall(int x, int y) - returns true if the cell is a wall
    void SetLight(int x, int y, float distanceSquared) - lights up the specified cell
  and two properties:
    int xDim, yDim - grid dimensions in the X and Y directions
  Coordinates are range-checked, so the methods will only be called with valid values.

- Uses a trivial "IntVector2" class, which just holds a pair of integers.
  If you lack something similar, just convert it to a pair of ints.
*/

namespace SatiRogue.Tools;

public static class ShadowCast {
   static readonly OctantTransform[] SOctantTransform = {
      new(1, 0, 0, 1), // 0 E-NE
      new(0, 1, 1, 0), // 1 NE-N
      new(0, -1, 1, 0), // 2 N-NW
      new(-1, 0, 0, 1), // 3 NW-W
      new(-1, 0, 0, -1), // 4 W-SW
      new(0, -1, -1, 0), // 5 SW-S
      new(0, 1, -1, 0), // 6 S-SE
      new(1, 0, 0, -1) // 7 SE-E
   };

   /// <summary>
   ///    Lights up cells visible from the current position.  Clear all lighting before calling.
   /// </summary>
   /// <param name="grid">The cell grid definition.</param>
   /// <param name="gridPosn">The player's position within the grid.</param>
   /// <param name="viewRadius">Maximum view distance; can be a fractional value.</param>
   public static void ComputeVisibility(MapGenData grid, Vector3 gridPosn, float viewRadius) {
      //Debug.Assert(gridPosn.x >= 0 && gridPosn.x < grid.xDim);
      //Debug.Assert(gridPosn.y >= 0 && gridPosn.y < grid.yDim);

      // Viewer's cell is always visible.
      grid.SetLight(gridPosn, 0.0f);

      // Cast light into cells for each of 8 octants.
      //
      // The left/right inverse slope values are initially 1 and 0, indicating a diagonal
      // and a horizontal line.  These aren't strictly correct, as the view area is supposed
      // to be based on corners, not center points.  We only really care about one side of the
      // wall at the edges of the octant though.
      //
      // NOTE: depending on the compiler, it's possible that passing the octant transform
      // values as four integers rather than an object reference would speed things up.
      // It's much tidier this way though.
      foreach (var t in SOctantTransform)
         CastLight(grid, gridPosn, viewRadius, 1, 1.0f, 0.0f, t);
   }

   /// <summary>
   ///    Recursively casts light into cells.  Operates on a single octant.
   /// </summary>
   /// <param name="grid">The cell grid definition.</param>
   /// <param name="gridPosn">The player's position within the grid.</param>
   /// <param name="viewRadius">The view radius; can be a fractional value.</param>
   /// <param name="startColumn">Current column; pass 1 as initial value.</param>
   /// <param name="leftViewSlope">
   ///    Slope of the left (upper) view edge; pass 1.0 as
   ///    the initial value.
   /// </param>
   /// <param name="rightViewSlope">
   ///    Slope of the right (lower) view edge; pass 0.0 as
   ///    the initial value.
   /// </param>
   /// <param name="txfrm">Coordinate multipliers for the octant transform.</param>
   /// Maximum recursion depth is (Ceiling(viewRadius)).
   static void CastLight(MapGenData grid,
      Vector3 gridPosn,
      float viewRadius,
      int startColumn,
      float leftViewSlope,
      float rightViewSlope,
      OctantTransform txfrm) {
      //Debug.Assert(leftViewSlope >= rightViewSlope);

      // Used for distance test.
      var viewRadiusSq = viewRadius * viewRadius;

      var viewCeiling = (int) Math.Ceiling(viewRadius);

      // Set true if the previous cell we encountered was blocked.
      var prevWasBlocked = false;

      // As an optimization, when scanning past a block we keep track of the
      // rightmost corner (bottom-right) of the last one seen.  If the next cell
      // is empty, we can use this instead of having to compute the top-right corner
      // of the empty cell.
      float savedRightSlope = -1;

      var xDim = grid.GeneratorParameters.Width;
      var yDim = grid.GeneratorParameters.Height;

      // Outer loop: walk across each column, stopping when we reach the visibility limit.
      for (var currentCol = startColumn; currentCol <= viewCeiling; currentCol++) {
         var xc = currentCol;

         // Inner loop: walk down the current column.  We start at the top, where X==Y.
         //
         // TODO: we waste time walking across the entire column when the view area
         //   is narrow.  Experiment with computing the possible range of cells from
         //   the slopes, and iterate over that instead.
         for (var yc = currentCol; yc >= 0; yc--) {
            // Translate local coordinates to grid coordinates.  For the various octants
            // we need to invert one or both values, or swap X for Y.
            var gridX = gridPosn.x + xc * txfrm.Xx + yc * txfrm.Xy;
            var gridY = gridPosn.z + xc * txfrm.Yx + yc * txfrm.Yy;

            // Range-check the values.  This lets us avoid the slope division for blocks
            // that are outside the grid.
            //
            // Note that, while we will stop at a solid column of blocks, we do always
            // start at the top of the column, which may be outside the grid if we're (say)
            // checking the first octant while positioned at the north edge of the map.
            if (gridX < 0 || gridX >= xDim || gridY < 0 || gridY >= yDim) continue;

            // Compute slopes to corners of current block.  We use the top-left and
            // bottom-right corners.  If we were iterating through a quadrant, rather than
            // an octant, we'd need to flip the corners we used when we hit the midpoint.
            //
            // Note these values will be outside the view angles for the blocks at the
            // ends -- left value > 1, right value < 0.
            var leftBlockSlope = (yc + 0.5f) / (xc - 0.5f);
            var rightBlockSlope = (yc - 0.5f) / (xc + 0.5f);

            // Check to see if the block is outside our view area.  Note that we allow
            // a "corner hit" to make the block visible.  Changing the tests to >= / <=
            // will reduce the number of cells visible through a corner (from a 3-wide
            // swath to a single diagonal line), and affect how far you can see past a block
            // as you approach it.  This is mostly a matter of personal preference.
            if (rightBlockSlope > leftViewSlope) // Block is above the left edge of our view area; skip.
               continue;

            if (leftBlockSlope < rightViewSlope) // Block is below the right edge of our view area; we're done.
               break;

            // This cell is visible, given infinite vision range.  If it's also within
            // our finite vision range, light it up.
            //
            // To avoid having a single lit cell poking out N/S/E/W, use a fractional
            // viewRadius, e.g. 8.5.
            //
            // TODO: we're testing the middle of the cell for visibility.  If we tested
            //  the bottom-left corner, we could say definitively that no part of the
            //  cell is visible, and reduce the view area as if it were a wall.  This
            //  could reduce iteration at the corners.
            float distanceSquared = xc * xc + yc * yc;
            var gridVec = new Vector3(gridX, 0, gridY);
            if (distanceSquared <= viewRadiusSq) grid.SetLight(gridVec, distanceSquared);

            var curBlocked = grid.IsWall(gridVec);

            if (prevWasBlocked) {
               if (curBlocked) {
                  // Still traversing a column of walls.
                  savedRightSlope = rightBlockSlope;
               } else {
                  // Found the end of the column of walls.  Set the left edge of our
                  // view area to the right corner of the last wall we saw.
                  prevWasBlocked = false;
                  leftViewSlope = savedRightSlope;
               }
               grid.SetLight(gridVec + Vector3.Back, distanceSquared);
            } else {
               if (curBlocked) {
                  // Found a wall.  Split the view area, recursively pursuing the
                  // part to the left.  The leftmost corner of the wall we just found
                  // becomes the right boundary of the view area.
                  //
                  // If this is the first block in the column, the slope of the top-left
                  // corner will be greater than the initial view slope (1.0).  Handle
                  // that here.
                  if (leftBlockSlope <= leftViewSlope)
                     CastLight(grid, gridPosn, viewRadius, currentCol + 1, leftViewSlope, leftBlockSlope, txfrm);

                  // Once that's done, we keep searching to the right (down the column),
                  // looking for another opening.
                  prevWasBlocked = true;
                  savedRightSlope = rightBlockSlope;
                  grid.SetLight(gridVec + Vector3.Back, distanceSquared);
               }
            }
         }

         // Open areas are handled recursively, with the function continuing to search to
         // the right (down the column).  If we reach the bottom of the column without
         // finding an open cell, then the area defined by our view area is completely
         // obstructed, and we can stop working.
         if (prevWasBlocked) break;
      }
   }

   /// <summary>
   ///    Immutable class for holding coordinate transform constants.  Bulkier than a 2D
   ///    array of ints, but it's self-formatting if you want to log it while debugging.
   /// </summary>
   class OctantTransform {
      public OctantTransform(int xx, int xy, int yx, int yy) {
         Xx = xx;
         Xy = xy;
         Yx = yx;
         Yy = yy;
      }

      public int Xx { get; }
      public int Xy { get; }
      public int Yx { get; }
      public int Yy { get; }

      public override string ToString() {
         // consider formatting in constructor to reduce garbage
         return string.Format("[OctantTransform {0,2:D} {1,2:D} {2,2:D} {3,2:D}]", Xx, Xy, Yx, Yy);
      }
   }
}