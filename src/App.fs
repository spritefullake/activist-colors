module App

open Fable.Core
open Fable.Core.JsInterop
open Fable.Import
open System
open Fable.React

open Elmish
open Elmish.React

open Feliz

type attr = Feliz.prop

//open Fable.React
//open Fable.React.Props

open Colors
open Polar
open Drawing
open Shape


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
printfn "Gride Size Steps: %i" steps

ctx.lineWidth <- 2.0


type PickerProps =
| Colors of string[] //it's crucial to pass these as an Array NOT List
| OnSwatchHover of (string -> Browser.Types.Event -> unit)
| OnChange of (Browser.Types.Event -> unit)

type IColor =
    //[<Emit("$0({color: #f7f7f7})")>]
    abstract BlockPicker : unit -> ReactElement

(*
[<ImportMember("react-color")>]
type BlockPicker =
    class
        new(arg) = BlockPicker(arg)
    end
*)

//let BlockPicker = ofImport "BlockPicker" "react-color"  [] []

//let BlockPicker = importMember "react-color"

let inline BlockPicker props : ReactElement =
    ofImport "BlockPicker" "react-color" (keyValueList CaseRules.LowerFirst props) []

type State =
    { Colors: list<Colors.Color>; ActiveColor: Colors.Color }

type Msg =
    | Add of Colors.Color
    | Remove of Colors.Color
    | SetActiveHex of string

let init() = { Colors = activismColors |> Seq.toList; ActiveColor = {name = "Color"; hex = "#aaaeee"} }

let dropFirst predicate items = 
    let rec loop validated items =                   
        match items with
        | [] -> []
        | x::xs -> 
            if predicate x then
                loop (validated@[x]) xs
            else
                validated@xs
    loop [] items    

let update (msg: Msg) (state: State): State =
    match msg with
    | Add color -> 
        { state with Colors = state.Colors@[color] }

    | Remove color -> 
        { state with Colors = dropFirst (fun c -> color <> c) state.Colors }
    
    | SetActiveHex hex ->
        { state with ActiveColor = {name = (sprintf "Color %d" (List.length state.Colors)); hex = hex}}

let render {Colors = colors; ActiveColor = active} (dispatch: Msg -> unit) =

    let colorDisplay color =
        [ Html.p color.name
          Html.div
              [ attr.className "color-box"
                attr.style [ style.backgroundColor color.hex ] 
                attr.onClick (fun _ -> (Remove >> dispatch) color)
                ] ]

    let displayedColors = Seq.collect colorDisplay colors

    let blockColors = (List.map (fun c -> c.hex) colors) |> List.toArray |> Colors
    let swatch = OnSwatchHover (fun color event -> Browser.Dom.console.log ("Color ", color, " hovered via ", event))
    let change = OnChange (fun event -> Browser.Dom.console.log ("Change via : ", event))
    Html.div
        [ Html.h1 "Colors List"
          Html.div
              [ attr.className "colors-list"
                attr.children displayedColors ]
          BlockPicker [ blockColors; swatch; change ]

          Html.h2 "Add a Color!"
          Html.button [ 
            attr.text "Add Color" 
            attr.onClick (fun _ -> (Add >> dispatch) active)
          ]
          Html.input [ 
              attr.type' "color"
              attr.className "color-picker"
              attr.valueOrDefault active.hex
              attr.onChange (SetActiveHex >> dispatch)
            ] ]

open Elmish.HMR
Program.mkSimple init update render
|> Program.withReactBatched "elmish-app"
|> Program.run



let origin = (200., 200.)
let angles = [ 0.0 .. Math.PI / 2. .. Math.PI * 2. ]
let zipped = Seq.zip angles activismHexes



for angle, color in zipped do
    rectangularCut 100. 100.
    |> transform (rotate angle)
    //|> Seq.iter (fun x -> ctx.stroke())
    |> movementTrack (Continuous origin)
    |> Seq.map unwrap
    |> Shape.fill ctx !^color

// write Fable
ctx.textAlign <- "center"
ctx.fillText ("What is this", gridWidth * 0.5, gridWidth * 0.5)

printfn "done!"
printfn "w: %f" w
