Img2FEN

Model to take an typical book image of a chessboard and return the likely FEN for that position.

It performs four steps:

- identifies the board in the given image - can be told to look either for a single board or a 2x3 page of boards
- divides each found board into 64 square regions, and normalizes that image area
- runs a model to identify the likely piece on that square (if any)
- assembles the returned array of 64 pieces into a valid FEN

Most of the work is pure OpenCV hacks, but piece identity is a trained ML model.

