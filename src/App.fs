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

let reset (ctx: Context) (x, y) = ctx.moveTo (x, y)

let drawReduce (ctx : Context) positions vector = 
  let position = Seq.head positions
  //printfn "Position is %f %f " <|| position
  match vector with
    | Continuous v ->
        let final = endPoint position v
        final
        |> ctx.lineTo

        Seq.append [final] positions 
    | Discontinuous v ->
        let final = endPoint position v
        final
        |> ctx.moveTo

        Seq.append [final] positions 

let fillShape (ctx: Context) color points  =
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
let angles = [0.0 .. Math.PI / 2. .. Math.PI * 2.]
let zipped = Seq.zip angles activismHexes

for angle, color in zipped do
  rectangularCut 300. 300.
  |> transform (rotate angle)
  |> mapTrack origin
  |> fillShape ctx !^color

// write Fable
ctx.textAlign <- "center"
ctx.fillText ("What is this", gridWidth * 0.5, gridWidth * 0.5)

printfn "done!"
printfn "w: %f" w
