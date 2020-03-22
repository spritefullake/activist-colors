module Drawing

open Polar
open Movement
open System

type Instruction = Movement<Polar>
type Drawing<'a> = Movement<'a> seq

let applyMovement (start : Movement<Point>)= 
  map << unwrap <| (map endPoint start)

let reify initial concreteFn movements : Drawing<'a> = 
  movements 
  |> track (predict concreteFn) (Seq.singleton initial)

let toPointDrawing initial = reify initial applyMovement

let buildRectangles centers dimensions =   
  //Rectangles are built clockwise from quad 4
  //According to the left-handed coordinate system
  let lengths = [
    for theta in 0.0 .. Math.PI / 2. .. Math.PI * 2. do
      rotation2D dimensions theta
  ]
  Seq.zip centers lengths

let rectangularCut length width =
  let turnAngle = Math.PI / 2. 
  let sides = 4
  seq {
    for i in 0..sides do
      if i % 2 = 0 then
        Continuous {radius = length; angle = turnAngle * (float i)}
      else 
        Continuous {radius = width; angle = turnAngle * (float i)}
  }

let rotationalCut (sections: int) length =
    let turnAngle: radians = 2. * Math.PI
    let sections = float sections
    let anglePerSection = turnAngle / sections

    let empty = fun i -> { radius = -length; angle = i * anglePerSection }

    let cut = fun i -> { radius = length; angle = i * anglePerSection }

    seq {
        for i in 0.0 .. sections do
            yield! seq {
                       Continuous (cut i)
                       Discontinuous (empty i)
                   }
    }

let polygonalCut (sides : int) = rotationalCut sides >> Seq.filter isContinuous

let verticalCut (sections: int) spacing length =
    let cut = { radius = length; angle = Math.PI / 2. }
    let move = { radius = spacing; angle = 0.0 }
    let empty = { radius = -length; angle = Math.PI / 2.}
    seq {
        for i in 0.0 .. (float sections) do
            yield! seq {
                       Continuous cut 
                       Discontinuous empty
                       Discontinuous move
                   }
    }

let profileSquares origin =
    let angles = [ 0.0 .. Math.PI / 2. .. Math.PI * 2. ]
    angles |> Seq.map (fun angle ->
        rectangularCut 100. 100.
        |> transform (rotate angle)
        |> toPointDrawing (Continuous origin))