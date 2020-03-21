module Polar

type radians = float

//Tuple for rectangular coordinates
type Point = float * float
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

let predict futureFn acc x =
  let final = futureFn (Seq.head acc) x
  Seq.append [final] acc 

let track reducer start = Seq.fold reducer start

let rotation2D (x, y) theta : Point =
  let x1 = x * cos(theta) - y * sin(theta)
  let y1 = x * sin(theta) + y * cos(theta)
  (x1, y1)