module Shape

open Fable.Core.JsInterop
open Fable.Import
open System
open Drawing
open Polar
open Movement

type Context = Browser.Types.CanvasRenderingContext2D

let reset (ctx: Context) (x, y) = ctx.moveTo (x, y)

let fill (ctx: Context) color (reified : Drawing<Point>)  =
  ctx.beginPath()
  ctx.fillStyle <- color
  for point in reified do
    map ctx.lineTo point
  ctx.fill()

let drawCanvas (ctx : Context) (reified : Drawing<Point>) = 
    Seq.map (binMap ctx.lineTo ctx.moveTo) reified
    ctx.stroke()

let fillCanvas (ctx : Context) pointsTransform (colorCodes : string seq) = 
    ctx.clearRect(0.,0., ctx.canvas.clientWidth, ctx.canvas.clientHeight)
    for drawing, code in Seq.zip pointsTransform colorCodes do
        fill ctx !^code drawing
