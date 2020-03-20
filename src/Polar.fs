module Polar

type radians = float

//Tuple for rectangular coordinates
type Point = float * float
type Points = seq<Point>
type Polar = //Represents Polar Coordinates as a vector
    { radius: float
      angle: radians }

//Convert polar to rectangular coordinates
let rectangular polar : Point =
    match polar with
    | { radius = r; angle = θ } ->
        let x = r * cos (θ)
        let y = r * sin (θ)
        (x, y)

let rotate θ {radius = r; angle = a} =
    {radius = r; angle = a + θ}

//Add a polar vector to a point
let endPoint start polar : Point =
    let (x, y) = start
    let (dx, dy) = rectangular polar
    (x + dx, y + dy)

let updatePositions positions polar  : Points =
  let current = Seq.head positions
  let final = endPoint current polar
  Seq.append [final] positions 

let rotation2D (x, y) theta : Point =
  let x1 = x * cos(theta) - y * sin(theta)
  let y1 = x * sin(theta) + y * cos(theta)
  (x1, y1)