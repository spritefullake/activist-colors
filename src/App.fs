module App

open Fable.Core.JsInterop
open Fable.Import
open System

let window = Browser.Dom.window

// Get our canvas context
// As we'll see later, myCanvas is mutable hence the use of the mutable keyword
// the unbox keyword allows to make an unsafe cast. Here we assume that getElementById will return an HTMLCanvasElement
let mutable myCanvas: Browser.Types.HTMLCanvasElement = unbox window.document.getElementById "myCanvas"
// myCanvas is defined in public/index.html

type Context = Browser.Types.CanvasRenderingContext2D

type radians = float

type Polar =
    { radius: float
      angle: radians }

type Continuity =
    | Continuous of Polar
    | Discontinuous of Polar

let rectangular polar =
    match polar with
    | { radius = r; angle = θ } ->
        let x = r * cos (θ)
        let y = r * sin (θ)
        (x, y)

let endPoint polar startPoint =
    let (x, y) = startPoint
    let (dx, dy) = rectangular polar
    (x + dx, y + dy)

// Get the context
let ctx = myCanvas.getContext_2d()

// All these are immutables values
let w = myCanvas.width
let h = myCanvas.height
let steps = 20
let squareSize = 20

// gridWidth needs a float wo we cast tour int operation to a float using the float keyword
let gridWidth = float (steps * squareSize)

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

let isContinuous x = 
  match x with 
  | Continuous _ -> true
  | _ -> false

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

// resize our canvas to the size of our grid
// the arrow <- indicates we're mutating a value. It's a special operator in F#.
myCanvas.width <- gridWidth
myCanvas.height <- gridWidth

// print the grid size to our debugger console
printfn "%i" steps


let printMove vector position =
    match vector with
      | Continuous {radius = r; angle = a} ->
          printfn "REL: r = %f, a = %f \n" r a
          printfn "CONT: Destination is %f %f \n\n" <|| position
      | Discontinuous {radius = r; angle = a} ->
          printfn "REL: r = %f, a = %f \n" r a
          printfn "DIS: Destination is %f %f \n\n" <|| position
    position
 

let reset (ctx: Context) (x, y) = ctx.moveTo (x, y)

let drawDecide vector positions = 
  let position = Seq.head positions
  printfn "Position is %f %f " <|| position
  match vector with
    | Continuous { radius = r; angle = a } ->
        let final = position |> endPoint {radius = r; angle = a}
        final
        |> ctx.lineTo

        Seq.append [final] positions 
    | Discontinuous { radius = r; angle = a } ->
        let final = position |> endPoint {radius = r; angle = a}
        final
        |> ctx.moveTo

        Seq.append [final] positions 


let rec fillRect positions =
  match positions |> Seq.toList with
  |  [] -> ()
  |  _ -> 
      let points = Seq.take 3 positions
      ctx.beginPath()
      for point in points do
        ctx.lineTo(point)
      ctx.fillStyle <- !^"blue"
      ctx.fill()
      Seq.skip 2 positions |> fillRect

let drawSquare (ctx: Context) length colors =
  ctx.beginPath()
  let lengths = [
    (length, length)
    (length, -length)
    (-length, length)
    (-length, -length)
  ]
  let zipped = Seq.zip colors lengths
  for color, (l1,l2) in zipped do
    ctx.fillStyle <- color
    ctx.fillRect(200.,200.,l1,l2)
    


ctx.lineWidth <- 2.0

let origin = seq{ (200., 200.) }
let reducer position i =
  drawDecide i position
let rotations = rotationalCut 4 100.
rotations
|> Seq.fold reducer origin
|> (fun x -> ctx.stroke(); x)
//|> Seq.length |> printfn "%d"

let profileColors = seq {
  !^"#DB0401"
  !^"#1B628E"
  !^"#56A1E4"
  !^"#56A1E4"
}
drawSquare ctx 50. profileColors

ctx.strokeStyle <- !^"red"
ctx.stroke()
// write Fable
ctx.textAlign <- "center"
ctx.fillText ("What is this", gridWidth * 0.5, gridWidth * 0.5)

printfn "done!"
printfn "w: %f" w
