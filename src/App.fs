module App

open Fable.Core.JsInterop
open Fable.Import
open System

open Colors
open Polar
open Drawing

let window = Browser.Dom.window

// Get our canvas context
// As we'll see later, myCanvas is mutable hence the use of the mutable keyword
// the unbox keyword allows to make an unsafe cast. Here we assume that getElementById will return an HTMLCanvasElement
let mutable myCanvas: Browser.Types.HTMLCanvasElement = unbox window.document.getElementById "myCanvas"
// myCanvas is defined in public/index.html

type Context = Browser.Types.CanvasRenderingContext2D

// Get the context
let ctx = myCanvas.getContext_2d()

// All these are immutables values
let w = myCanvas.width
let h = myCanvas.height
let steps = 20
let squareSize = 20

// gridWidth needs a float wo we cast tour int operation to a float using the float keyword
let gridWidth = float (steps * squareSize)


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


let computePositions positions v =
  let final = positions |> Seq.head >> endPoint v
  Seq.append [final] positions 

let compMatch positions  (Continuous v | Discontinuous v) =
  let final = (Seq.head positions) |> endPoint v
  Seq.append [final] positions 

let drawReduce (ctx : Context) positions vector = 
  let position = Seq.head positions
  //printfn "Position is %f %f " <|| position
  match vector with
    | Continuous v ->
        let final = position |> endPoint v
        final
        |> ctx.lineTo

        Seq.append [final] positions 
    | Discontinuous v ->
        let final = position |> endPoint v
        final
        |> ctx.moveTo

        Seq.append [final] positions 

let fillTriangle (ctx: Context) color points  =
  ctx.beginPath()
  ctx.fillStyle <- color
  for point in points do
    ctx.lineTo(point)
  ctx.fill()

let drawRectangles (ctx: Context) center dimensions (colors : seq<String>) =
  ctx.beginPath()
  let built = buildRectangles center dimensions
  let zipped = Seq.zip colors built
  for color, ((x, y), (width, height)) in zipped do
    ctx.fillStyle <- !^color
    ctx.fillRect(x, y, width, height)

let drawSquares (ctx: Context) center length = 
  drawRectangles ctx center (length, length)

ctx.lineWidth <- 2.0

let origin = (200., 200.) 

let rotations = rotationalCut 4 100.


//drawSquares ctx [for i in 0..4 do origin] 100. activismHexes

ctx.fillStyle <- !^"green"
let rects = 
  rectangularCut 100. 100. 
  |> Seq.fold (drawReduce ctx) (Seq.singleton origin)
  |> printfn "RECTS: %A"


let origins = [
  (200.,200.)
  
  (100.,200.)
  (200.,100.)
  (100.,100.)
]
let zipped = Seq.zip origins activismHexes
for o, color in zipped do 
  rectangularCut 100. 100. 
  |> Seq.fold compMatch (Seq.singleton o)
  |> fillTriangle ctx !^color


ctx.stroke()

// write Fable
ctx.textAlign <- "center"
ctx.fillText ("What is this", gridWidth * 0.5, gridWidth * 0.5)

printfn "done!"
printfn "w: %f" w
