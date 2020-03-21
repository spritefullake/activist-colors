module Shape

open Fable.Core.JsInterop
open Fable.Import
open System
open Drawing
open Polar

type Context = Browser.Types.CanvasRenderingContext2D

let reset (ctx: Context) (x, y) = ctx.moveTo (x, y)

let fill (ctx: Context) color points  =
  ctx.beginPath()
  ctx.fillStyle <- color
  for point in points do
    ctx.lineTo(point)
  ctx.fill()

let drawCanvas (ctx : Context) points = 
    Seq.map (binMap ctx.lineTo ctx.moveTo) points
    ctx.stroke()
