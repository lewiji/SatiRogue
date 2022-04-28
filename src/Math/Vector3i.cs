using System;
using Godot;

namespace RoguelikeMono.Math;

[Serializable]
public struct Vector3i : IEquatable<Vector3i>
{
  /// <summary>
    /// The vector's X component. Also accessible by using the index position <c>[0]</c>.
    /// </summary>
    public int x;
    /// <summary>
    /// The vector's Y component. Also accessible by using the index position <c>[1]</c>.
    /// </summary>
    public int y;
    /// <summary>
    /// The vector's Z component. Also accessible by using the index position <c>[2]</c>.
    /// </summary>
    public int z;
    private static readonly Vector3i _zero = new Vector3i(0, 0, 0);
    private static readonly Vector3i _one = new Vector3i(1, 1, 1);
    private static readonly Vector3i _negOne = new Vector3i(-1, -1, -1);
    private static readonly Vector3i _inf = new Vector3i(Int32.MaxValue, Int32.MaxValue, Int32.MaxValue);
    private static readonly Vector3i _up = new Vector3i(0, 1, 0);
    private static readonly Vector3i _down = new Vector3i(0, -1, 0);
    private static readonly Vector3i _right = new Vector3i(1, 0, 0);
    private static readonly Vector3i _left = new Vector3i(-1, 0, 0);
    private static readonly Vector3i _forward = new Vector3i(0, 0, -1);
    private static readonly Vector3i _back = new Vector3i(0, 0, 1);

    public Vector3 ToVector3()
    {
      return new Vector3(x, y, z);
    }

    /// <summary>Access vector components using their index.</summary>
    /// <exception cref="T:System.IndexOutOfRangeException">
    /// Thrown when the given the <paramref name="index" /> is not 0, 1 or 2.
    /// </exception>
    /// <value>
    /// <c>[0]</c> is equivalent to <see cref="F:Godot.Vector3i.x" />,
    /// <c>[1]</c> is equivalent to <see cref="F:Godot.Vector3i.y" />,
    /// <c>[2]</c> is equivalent to <see cref="F:Godot.Vector3i.z" />.
    /// </value>
    public int this[int index]
    {
      get
      {
        switch (index)
        {
          case 0:
            return this.x;
          case 1:
            return this.y;
          case 2:
            return this.z;
          default:
            throw new IndexOutOfRangeException();
        }
      }
      set
      {
        switch (index)
        {
          case 0:
            this.x = value;
            break;
          case 1:
            this.y = value;
            break;
          case 2:
            this.z = value;
            break;
          default:
            throw new IndexOutOfRangeException();
        }
      }
    }

    internal void Normalize()
    {
      int s = this.LengthSquared();
      if ((double) s == 0.0)
      {
        this.x = this.y = this.z = 0;
      }
      else
      {
        var num = Mathf.Sqrt(s);
        x = (int)(x / num);
        y = (int)(y / num);
        z = (int)(z / num);
      }
    }

    /// <summary>
    /// Returns a new vector with all components in absolute values (i.e. positive).
    /// </summary>
    /// <returns>A vector with <see cref="M:Godot.Mathf.Abs(System.Single)" /> called on each component.</returns>
    public Vector3i Abs() => new Vector3i(Mathf.Abs(this.x), Mathf.Abs(this.y), Mathf.Abs(this.z));

    /// <summary>
    /// Returns the unsigned minimum angle to the given vector, in radians.
    /// </summary>
    /// <param name="to">The other vector to compare this vector to.</param>
    /// <returns>The unsigned angle between the two vectors, in radians.</returns>
    public float AngleTo(Vector3i to) => Mathf.Atan2(this.Cross(to).Length(), this.Dot(to));

    /// <summary>
    /// Returns this vector "bounced off" from a plane defined by the given normal.
    /// </summary>
    /// <param name="normal">The normal vector defining the plane to bounce off. Must be normalized.</param>
    /// <returns>The bounced vector.</returns>
    public Vector3i Bounce(Vector3i normal) => -this.Reflect(normal);

    /// <summary>
    /// Returns a new vector with all components rounded up (towards positive infinity).
    /// </summary>
    /// <returns>A vector with <see cref="M:Godot.Mathf.CeilToInt(System.Single)" /> called on each component.</returns>
    public Vector3i Ceil() => new Vector3i(Mathf.CeilToInt(this.x), Mathf.CeilToInt(this.y), Mathf.CeilToInt(this.z));

    /// <summary>
    /// Returns the cross product of this vector and <paramref name="b" />.
    /// </summary>
    /// <param name="b">The other vector.</param>
    /// <returns>The cross product vector.</returns>
    public Vector3i Cross(Vector3i b) => new Vector3i((int) ((double) this.y * (double) b.z - (double) this.z * (double) b.y), (int) ((double) this.z * (double) b.x - (double) this.x * (double) b.z), (int) ((double) this.x * (double) b.y - (double) this.y * (double) b.x));

    /// <summary>
    /// Performs a cubic interpolation between vectors <paramref name="preA" />, this vector,
    /// <paramref name="b" />, and <paramref name="postB" />, by the given amount <paramref name="weight" />.
    /// </summary>
    /// <param name="b">The destination vector.</param>
    /// <param name="preA">A vector before this vector.</param>
    /// <param name="postB">A vector after <paramref name="b" />.</param>
    /// <param name="weight">A value on the range of 0.0 to 1.0, representing the amount of interpolation.</param>
    /// <returns>The interpolated vector.</returns>
    public Vector3i CubicInterpolate(Vector3i b, Vector3i preA, Vector3i postB, int weight)
    {
      Vector3i Vector3i_1 = preA;
      Vector3i Vector3i_2 = this;
      Vector3i Vector3i_3 = b;
      Vector3i Vector3i_4 = postB;
      int num1 = weight;
      int num2 = num1 * num1;
      int num3 = num2 * num1;
      return 0 * (Vector3i_2 * 2 + (-Vector3i_1 + Vector3i_3) * num1 + (2 * Vector3i_1 - 5 * Vector3i_2 + 4 * Vector3i_3 - Vector3i_4) * num2 + (-Vector3i_1 + 3 * Vector3i_2 - 3 * Vector3i_3 + Vector3i_4) * num3);
    }

    /// <summary>
    /// Returns the normalized vector pointing from this vector to <paramref name="b" />.
    /// </summary>
    /// <param name="b">The other vector to point towards.</param>
    /// <returns>The direction from this vector to <paramref name="b" />.</returns>
    public Vector3i DirectionTo(Vector3i b) => new Vector3i(b.x - this.x, b.y - this.y, b.z - this.z).Normalized();

    /// <summary>
    /// Returns the squared distance between this vector and <paramref name="b" />.
    /// This method runs faster than <see cref="M:Godot.Vector3i.DistanceTo(Godot.Vector3i)" />, so prefer it if
    /// you need to compare vectors or need the squared distance for some formula.
    /// </summary>
    /// <param name="b">The other vector to use.</param>
    /// <returns>The squared distance between the two vectors.</returns>
    public int DistanceSquaredTo(Vector3i b) => (b - this).LengthSquared();

    /// <summary>
    /// Returns the distance between this vector and <paramref name="b" />.
    /// </summary>
    /// <seealso cref="M:Godot.Vector3i.DistanceSquaredTo(Godot.Vector3i)" />
    /// <param name="b">The other vector to use.</param>
    /// <returns>The distance between the two vectors.</returns>
    public float DistanceTo(Vector3i b) => (b - this).Length();

    /// <summary>
    /// Returns the dot product of this vector and <paramref name="b" />.
    /// </summary>
    /// <param name="b">The other vector to use.</param>
    /// <returns>The dot product of the two vectors.</returns>
    public int Dot(Vector3i b) => (int) ((double) this.x * (double) b.x + (double) this.y * (double) b.y + (double) this.z * (double) b.z);

    /// <summary>
    /// Returns a new vector with all components rounded down (towards negative infinity).
    /// </summary>
    /// <returns>A vector with <see cref="M:Godot.Mathf.FloorToInt(System.Single)" /> called on each component.</returns>
    public Vector3i Floor() => new Vector3i(Mathf.FloorToInt(this.x), Mathf.FloorToInt(this.y), Mathf.FloorToInt(this.z));

    /// <summary>
    /// Returns the inverse of this vector. This is the same as <c>new Vector3i(1 / v.x, 1 / v.y, 1 / v.z)</c>.
    /// </summary>
    /// <returns>The inverse of this vector.</returns>
    public Vector3i Inverse() => new Vector3i(1 / this.x, 1 / this.y, 1 / this.z);

    /// <summary>
    /// Returns <see langword="true" /> if the vector is normalized, and <see langword="false" /> otherwise.
    /// </summary>
    /// <returns>A <see langword="bool" /> indicating whether or not the vector is normalized.</returns>
    public bool IsNormalized() => (double) Mathf.Abs(this.LengthSquared() - 1) < 9.999999974752427E-07;

    /// <summary>Returns the length (magnitude) of this vector.</summary>
    /// <seealso cref="M:Godot.Vector3i.LengthSquared" />
    /// <returns>The length of this vector.</returns>
    public float Length() => Mathf.Sqrt(this.x * this.x + this.y * this.y + this.z * this.z);

    /// <summary>
    /// Returns the squared length (squared magnitude) of this vector.
    /// This method runs faster than <see cref="M:Godot.Vector3i.Length" />, so prefer it if
    /// you need to compare vectors or need the squared length for some formula.
    /// </summary>
    /// <returns>The squared length of this vector.</returns>
    public int LengthSquared() => this.x * this.x + this.y * this.y + this.z * this.z;
    
    /// <summary>
    /// Returns the axis of the vector's largest value. See <see cref="T:Godot.Vector3i.Axis" />.
    /// If all components are equal, this method returns <see cref="F:Godot.Vector3i.Axis.X" />.
    /// </summary>
    /// <returns>The index of the largest axis.</returns>
    public Vector3i.Axis MaxAxis() => (double) this.x < (double) this.y ? ((double) this.y < (double) this.z ? Vector3i.Axis.Z : Vector3i.Axis.Y) : ((double) this.x < (double) this.z ? Vector3i.Axis.Z : Vector3i.Axis.X);

    /// <summary>
    /// Returns the axis of the vector's smallest value. See <see cref="T:Godot.Vector3i.Axis" />.
    /// If all components are equal, this method returns <see cref="F:Godot.Vector3i.Axis.Z" />.
    /// </summary>
    /// <returns>The index of the smallest axis.</returns>
    public Vector3i.Axis MinAxis() => (double) this.x < (double) this.y
      ? ((double) this.x < (double) this.z ? Vector3i.Axis.X : Vector3i.Axis.Z)
      : ((double) this.y < (double) this.z ? Vector3i.Axis.Y : Vector3i.Axis.Z);

    /// <summary>
    /// Returns the vector scaled to unit length. Equivalent to <c>v / v.Length()</c>.
    /// </summary>
    /// <returns>A normalized version of the vector.</returns>
    public Vector3i Normalized()
    {
      Vector3i Vector3i = this;
      Vector3i.Normalize();
      return Vector3i;
    }

    /// <summary>
    /// Returns a vector composed of the <see cref="M:Godot.Mathf.PosMod(System.Single,System.Single)" /> of this vector's components
    /// and <paramref name="mod" />.
    /// </summary>
    /// <param name="mod">A value representing the divisor of the operation.</param>
    /// <returns>
    /// A vector with each component <see cref="M:Godot.Mathf.PosMod(System.Single,System.Single)" /> by <paramref name="mod" />.
    /// </returns>
    public Vector3i PosMod(int mod)
    {
      Vector3i Vector3i;
      Vector3i.x = Mathf.PosMod(this.x, mod);
      Vector3i.y = Mathf.PosMod(this.y, mod);
      Vector3i.z = Mathf.PosMod(this.z, mod);
      return Vector3i;
    }

    /// <summary>
    /// Returns a vector composed of the <see cref="M:Godot.Mathf.PosMod(System.Single,System.Single)" /> of this vector's components
    /// and <paramref name="modv" />'s components.
    /// </summary>
    /// <param name="modv">A vector representing the divisors of the operation.</param>
    /// <returns>
    /// A vector with each component <see cref="M:Godot.Mathf.PosMod(System.Single,System.Single)" /> by <paramref name="modv" />'s components.
    /// </returns>
    public Vector3i PosMod(Vector3i modv)
    {
      Vector3i Vector3i;
      Vector3i.x = Mathf.PosMod(this.x, modv.x);
      Vector3i.y = Mathf.PosMod(this.y, modv.y);
      Vector3i.z = Mathf.PosMod(this.z, modv.z);
      return Vector3i;
    }

    /// <summary>
    /// Returns this vector projected onto another vector <paramref name="onNormal" />.
    /// </summary>
    /// <param name="onNormal">The vector to project onto.</param>
    /// <returns>The projected vector.</returns>
    public Vector3i Project(Vector3i onNormal) => onNormal * (this.Dot(onNormal) / onNormal.LengthSquared());

    /// <summary>
    /// Returns this vector reflected from a plane defined by the given <paramref name="normal" />.
    /// </summary>
    /// <param name="normal">The normal vector defining the plane to reflect from. Must be normalized.</param>
    /// <returns>The reflected vector.</returns>
    public Vector3i Reflect(Vector3i normal)
    {
      if (!normal.IsNormalized())
        throw new ArgumentException("Argument is not normalized", nameof (normal));
      return 2 * this.Dot(normal) * normal - this;
    }

    /// <summary>
    /// Returns this vector with all components rounded to the nearest integer,
    /// with halfway cases rounded towards the nearest multiple of two.
    /// </summary>
    /// <returns>The rounded vector.</returns>
    public Vector3i Round() => new Vector3i(Mathf.RoundToInt(this.x), Mathf.RoundToInt(this.y), Mathf.RoundToInt(this.z));

    [Obsolete("Set is deprecated. Use the Vector3i(real_t, real_t, real_t) constructor instead.", true)]
    public void Set(int x, int y, int z)
    {
      this.x = x;
      this.y = y;
      this.z = z;
    }

    [Obsolete("Set is deprecated. Use the Vector3i(Vector3i) constructor instead.", true)]
    public void Set(Vector3i v)
    {
      this.x = v.x;
      this.y = v.y;
      this.z = v.z;
    }

    /// <summary>
    /// Returns a vector with each component set to one or negative one, depending
    /// on the signs of this vector's components, or zero if the component is zero,
    /// by calling <see cref="M:Godot.Mathf.Sign(System.Single)" /> on each component.
    /// </summary>
    /// <returns>A vector with all components as either <c>1</c>, <c>-1</c>, or <c>0</c>.</returns>
    public Vector3i Sign()
    {
      Vector3i Vector3i;
      Vector3i.x = (int) Mathf.Sign(this.x);
      Vector3i.y = (int) Mathf.Sign(this.y);
      Vector3i.z = (int) Mathf.Sign(this.z);
      return Vector3i;
    }

    /// <summary>
    /// Returns this vector slid along a plane defined by the given <paramref name="normal" />.
    /// </summary>
    /// <param name="normal">The normal vector defining the plane to slide on.</param>
    /// <returns>The slid vector.</returns>
    public Vector3i Slide(Vector3i normal) => this - normal * this.Dot(normal);


    /// <summary>
    /// Zero vector, a vector with all components set to <c>0</c>.
    /// </summary>
    /// <value>Equivalent to <c>new Vector3i(0, 0, 0)</c>.</value>
    public static Vector3i Zero => Vector3i._zero;

    /// <summary>
    /// One vector, a vector with all components set to <c>1</c>.
    /// </summary>
    /// <value>Equivalent to <c>new Vector3i(1, 1, 1)</c>.</value>
    public static Vector3i One => Vector3i._one;

    /// <summary>
    /// Deprecated, please use a negative sign with <see cref="P:Godot.Vector3i.One" /> instead.
    /// </summary>
    /// <value>Equivalent to <c>new Vector3i(-1, -1, -1)</c>.</value>
    [Obsolete("Use a negative sign with Vector3i.One instead.")]
    public static Vector3i NegOne => Vector3i._negOne;

    /// <summary>
    /// Infinity vector, a vector with all components set to <see cref="F:Godot.Mathf.Inf" />.
    /// </summary>
    /// <value>Equivalent to <c>new Vector3i(Mathf.Inf, Mathf.Inf, Mathf.Inf)</c>.</value>
    public static Vector3i Inf => Vector3i._inf;

    /// <summary>Up unit vector.</summary>
    /// <value>Equivalent to <c>new Vector3i(0, 1, 0)</c>.</value>
    public static Vector3i Up => Vector3i._up;

    /// <summary>Down unit vector.</summary>
    /// <value>Equivalent to <c>new Vector3i(0, -1, 0)</c>.</value>
    public static Vector3i Down => Vector3i._down;

    /// <summary>
    /// Right unit vector. Represents the local direction of right,
    /// and the global direction of east.
    /// </summary>
    /// <value>Equivalent to <c>new Vector3i(1, 0, 0)</c>.</value>
    public static Vector3i Right => Vector3i._right;

    /// <summary>
    /// Left unit vector. Represents the local direction of left,
    /// and the global direction of west.
    /// </summary>
    /// <value>Equivalent to <c>new Vector3i(-1, 0, 0)</c>.</value>
    public static Vector3i Left => Vector3i._left;

    /// <summary>
    /// Forward unit vector. Represents the local direction of forward,
    /// and the global direction of north.
    /// </summary>
    /// <value>Equivalent to <c>new Vector3i(0, 0, -1)</c>.</value>
    public static Vector3i Forward => Vector3i._forward;

    /// <summary>
    /// Back unit vector. Represents the local direction of back,
    /// and the global direction of south.
    /// </summary>
    /// <value>Equivalent to <c>new Vector3i(0, 0, 1)</c>.</value>
    public static Vector3i Back => Vector3i._back;

    /// <summary>
    /// Constructs a new <see cref="T:Godot.Vector3i" /> with the given components.
    /// </summary>
    /// <param name="x">The vector's X component.</param>
    /// <param name="y">The vector's Y component.</param>
    /// <param name="z">The vector's Z component.</param>
    public Vector3i(int x, int y, int z)
    {
      this.x = x;
      this.y = y;
      this.z = z;
    }

    /// <summary>
    /// Constructs a new <see cref="T:Godot.Vector3i" /> from an existing <see cref="T:Godot.Vector3i" />.
    /// </summary>
    /// <param name="v">The existing <see cref="T:Godot.Vector3i" />.</param>
    public Vector3i(Vector3i v)
    {
      this.x = v.x;
      this.y = v.y;
      this.z = v.z;
    }

    public static Vector3i operator +(Vector3i left, Vector3i right)
    {
      left.x += right.x;
      left.y += right.y;
      left.z += right.z;
      return left;
    }

    public static Vector3i operator -(Vector3i left, Vector3i right)
    {
      left.x -= right.x;
      left.y -= right.y;
      left.z -= right.z;
      return left;
    }

    public static Vector3i operator -(Vector3i vec)
    {
      vec.x = -vec.x;
      vec.y = -vec.y;
      vec.z = -vec.z;
      return vec;
    }

    public static Vector3i operator *(Vector3i vec, int scale)
    {
      vec.x *= scale;
      vec.y *= scale;
      vec.z *= scale;
      return vec;
    }

    public static Vector3i operator *(int scale, Vector3i vec)
    {
      vec.x *= scale;
      vec.y *= scale;
      vec.z *= scale;
      return vec;
    }

    public static Vector3i operator *(Vector3i left, Vector3i right)
    {
      left.x *= right.x;
      left.y *= right.y;
      left.z *= right.z;
      return left;
    }

    public static Vector3i operator /(Vector3i vec, int divisor)
    {
      vec.x /= divisor;
      vec.y /= divisor;
      vec.z /= divisor;
      return vec;
    }

    public static Vector3i operator /(Vector3i vec, Vector3i divisorv)
    {
      vec.x /= divisorv.x;
      vec.y /= divisorv.y;
      vec.z /= divisorv.z;
      return vec;
    }

    public static Vector3i operator %(Vector3i vec, int divisor)
    {
      vec.x %= divisor;
      vec.y %= divisor;
      vec.z %= divisor;
      return vec;
    }

    public static Vector3i operator %(Vector3i vec, Vector3i divisorv)
    {
      vec.x %= divisorv.x;
      vec.y %= divisorv.y;
      vec.z %= divisorv.z;
      return vec;
    }

    public static bool operator ==(Vector3i left, Vector3i right) => left.Equals(right);

    public static bool operator !=(Vector3i left, Vector3i right) => !left.Equals(right);

    public static bool operator <(Vector3i left, Vector3i right)
    {
      if ((double) left.x != (double) right.x)
        return (double) left.x < (double) right.x;
      return (double) left.y == (double) right.y ? (double) left.z < (double) right.z : (double) left.y < (double) right.y;
    }

    public static bool operator >(Vector3i left, Vector3i right)
    {
      if ((double) left.x != (double) right.x)
        return (double) left.x > (double) right.x;
      return (double) left.y == (double) right.y ? (double) left.z > (double) right.z : (double) left.y > (double) right.y;
    }

    public static bool operator <=(Vector3i left, Vector3i right)
    {
      if ((double) left.x != (double) right.x)
        return (double) left.x < (double) right.x;
      return (double) left.y == (double) right.y ? (double) left.z <= (double) right.z : (double) left.y < (double) right.y;
    }

    public static bool operator >=(Vector3i left, Vector3i right)
    {
      if ((double) left.x != (double) right.x)
        return (double) left.x > (double) right.x;
      return (double) left.y == (double) right.y ? (double) left.z >= (double) right.z : (double) left.y > (double) right.y;
    }

    /// <summary>
    /// Returns <see langword="true" /> if this vector and <paramref name="obj" /> are equal.
    /// </summary>
    /// <param name="obj">The other object to compare.</param>
    /// <returns>Whether or not the vector and the other object are equal.</returns>
    public override bool Equals(object obj) => obj is Vector3i other && this.Equals(other);

    /// <summary>
    /// Returns <see langword="true" /> if this vector and <paramref name="other" /> are equal
    /// </summary>
    /// <param name="other">The other vector to compare.</param>
    /// <returns>Whether or not the vectors are equal.</returns>
    public bool Equals(Vector3i other) => (double) this.x == (double) other.x && (double) this.y == (double) other.y && (double) this.z == (double) other.z;

    /// <summary>
    /// Returns <see langword="true" /> if this vector and <paramref name="other" /> are approximately equal,
    /// by running <see cref="M:Godot.Mathf.IsEqualApprox(System.Single,System.Single)" /> on each component.
    /// </summary>
    /// <param name="other">The other vector to compare.</param>
    /// <returns>Whether or not the vectors are approximately equal.</returns>
    public bool IsEqualApprox(Vector3i other) => Mathf.IsEqualApprox(this.x, other.x) && Mathf.IsEqualApprox(this.y, other.y) && Mathf.IsEqualApprox(this.z, other.z);

    /// <summary>
    /// Serves as the hash function for <see cref="T:Godot.Vector3i" />.
    /// </summary>
    /// <returns>A hash code for this vector.</returns>
    public override int GetHashCode() => this.y.GetHashCode() ^ this.x.GetHashCode() ^ this.z.GetHashCode();

    /// <summary>
    /// Converts this <see cref="T:Godot.Vector3i" /> to a string.
    /// </summary>
    /// <returns>A string representation of this vector.</returns>
    public override string ToString() => String.Format("({0}, {1}, {2})", (object) this.x, (object) this.y, (object) this.z);

    /// <summary>
    /// Converts this <see cref="T:Godot.Vector3i" /> to a string with the given <paramref name="format" />.
    /// </summary>
    /// <returns>A string representation of this vector.</returns>
    public string ToString(string format) => "(" + this.x.ToString(format) + ", " + this.y.ToString(format) + ", " + this.z.ToString(format) + ")";

    /// <summary>
    /// Enumerated index values for the axes.
    /// Returned by <see cref="M:Godot.Vector3i.MaxAxis" /> and <see cref="M:Godot.Vector3i.MinAxis" />.
    /// </summary>
    public enum Axis
    {
      /// <summary>The vector's X axis.</summary>
      X,
      /// <summary>The vector's Y axis.</summary>
      Y,
      /// <summary>The vector's Z axis.</summary>
      Z,
    }
}