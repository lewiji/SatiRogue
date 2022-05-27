the assetpack contains unique fixed-size elements and tiled elements.

######################
UNIQUE SIZED ELEMENTS:
######################
The unique size elements come as versions placed on a 480x272 bg and as cropped sprite versions. Just use them like images. 
The folders also contain some small postits you can add to existing assepieces, if you want to.

###############
TILED ELEMENTS:
###############

The folder contains a sub-folder named "Tileset_Instructions" with detailled examples

The folder als contains 23 Tileset files:

tileset_booktabs: 		those are tabs which can be placed as sprites on top of the books to access different pages and come in 3 colors.
tileset_highlightspage: 	those are dark and bright versions for the flippe dove rpages for books. Use them as sprite to create a mouseover highlight.
tileset_paper_add-on_elements: 	those contain various colored add-ons for books and scrolls. Some of them are as colorless gray version in the main tilesets - just paste them on top of the respective tiles.
tileset_paper: 			those contain all the main tiles. In the folder "Tileset_Instructions" you find the file "a_tile_referencenames" and a bunch of examples
tileset_paper_seal_elements:	these contain all kinds of seals which should be placed on top of the gray seals on the main tileset (K3, P3) or can be used on their own

How to build a letter / book / scroll / paper from the tiles:

1.) Decide on a color scheme. There are 3 types of paper colors and 4 types of book colors in the "tileset_paper" files
2.) Decide which type of object you want to build. In the folder "Tileset_Instructions" there is an example for each type
3.) In order to understand the tileset you need to look at the example, see out of which elements it is made up and then find the position of those elements in the tileset.
	Best is to look at all 3 pictures (the instructions, the colored example and the info example) at a single screen.
4.) With the tileset you will be able to build out most of the graphic. Set a grid to 32px for easier cutting or if you want to directly create it in Unity, Gamemaker or a tile software like Tiled.
5.) Once your base is ready you can use "tileset_booktabs", "tileset_highlightspage", tileset_paper_add-on_elements" and "tileset_paper_seal_elements" as sprites to add colored details and buttons to your Ui. 
	If you use an imagesoftware to build your examples, just place them above the gray basic tiles, if you use them in an engine, it is most efficient to place all of them as unique sprites.
6.) Now you should have your very own graphic.

example scroll1 uses a seal placed without a gray base. It#s possible to place them like this, but maybe you want to paint in a shadow beneath it if you use them like that (the example has none)

I also want to point out that further color changes, or personal detail can greatly customize the graphic you just built.

---

Notes about the tile referencenames:

A1: 	empty middlepiece
B1-B4:	normal paper corners
B5:	variations left paper border
B6:	variations top paper border
B7:	variations bottom paper border
B8:	variations right paper border

C1-C4:	flipped paper corners (use with sprites "tileset_highlightspage")

D1:	flipped letter border, top left corner
D2:	flipped letter border, top right corner (2 tiles)
D6:	variations top flipped letter border
D7:	variations bottom flipped letter border

E1:	left side torn paper (fixed size only)

F1-F4:	normal burnt paper corners
F5:	variations left burnt paper border
F6:	variations top burnt paper border
F7:	variations bottom burnt paper border
F8:	variations right burnt paper border

G1-G4:	scroll1 horizontal - cornerpieces
H1-H4:	scroll2 horizontal - cornerpieces
I1-I4:	scroll3 horizontal - cornerpieces
J1-J4:	scroll4 horizontal - cornerpieces

K3:	horizontal scroll seal base (2 tiles high)
K5:	scroll horizontal - variations left border
K8:	scroll horizontal - variations right border

L1-L4:	scroll1 vertical - cornerpieces
M1-M4:	scroll2 vertical - cornerpieces
N1-N4:	scroll3 vertical - cornerpieces
O1-O4:	scroll4 vertical - cornerpieces

P3:	vertical scroll seal base (2 tiles wide)
P6:	scroll vertical - variations top border
P7:	scroll vertical - variations bottom border

Q1-Q4:	book1 - cornerpieces
Q5-Q8:	book1 - 2x flipped pages (dark and bright - use with sprites "tileset_highlightspage")
Q9:	book1 - middle top border
Q10:	book1 - middle bottom border

R5:	books - variations left border pieces
R6:	books - variations top border pieces
R7:	books - variations bottom border pieces
R8:	books - variations left border pieces

S1-S4:	book2 - cornerpieces
S5-S8:	book2 - 2x flipped pages (dark and bright - use with sprites "tileset_highlightspage")
S9:	book2 - middle top border
S10:	book2 - middle bottom border

T1-T4:	book3 - cornerpieces
T5-T8:	book3 - 2x flipped pages (dark and bright - use with sprites "tileset_highlightspage")
T9:	book3 - middle top border
T10:	book3 - middle bottom border