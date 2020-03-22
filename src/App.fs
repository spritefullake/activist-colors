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
printfn "Width: %f, height %f" w h

ctx.lineWidth <- 2.0


type PickerProps =
    | Colors of string [] //it's crucial to pass these as an Array NOT List
    | OnSwatchHover of (string -> Browser.Types.Event -> unit)
    | OnChange of (Browser.Types.Event -> unit)

type IColor =
    //[<Emit("$0({color: #f7f7f7})")>]
    abstract BlockPicker: unit -> ReactElement

(*
[<ImportMember("react-color")>]
type BlockPicker =
    class
        new(arg) = BlockPicker(arg)
    end
*)

//let BlockPicker = ofImport "BlockPicker" "react-color"  [] []

//let BlockPicker = importMember "react-color"

let inline BlockPicker props: ReactElement =
    ofImport "BlockPicker" "react-color" (keyValueList CaseRules.LowerFirst props) []

type State =
    { Colors: list<Colors.Color>
      ActiveColor: Colors.Color }

type Msg =
    | Add of Colors.Color
    | Remove of Colors.Color
    | SetActive of string
    | DrawProfile

let init() =
    { Colors = activismColors
      ActiveColor =
          { name = sprintf "Color %d" (distinctCodes activismColors)
            code = "#aaaeee"
            id = nextId activismColors } }, Cmd.ofMsg DrawProfile

let newColor code colors =
    let picker color =
        if color.code = code then Some color else None
    match List.tryPick picker colors with
    | Some color -> { color with id = nextId colors }
    | None ->
        { name = sprintf "Color %d" (distinctCodes colors)
          code = code
          id = nextId colors }

let refreshActiveColor state =
    let { ActiveColor = active; Colors = colors } = state
    { state with ActiveColor = { active with id = nextId colors } }




let paint colors =
    let pointsTransform = profileSquares (200., 200.)
    let colorCodes = getCodes colors
    fillCanvas ctx pointsTransform colorCodes

let update (msg: Msg) (state: State): State * Cmd<Msg> =
    let { Colors = colors } = state
    printfn "THE COLORS ARE %A" colors
    match msg with
    | Add color ->
        let updated = colors @ [ color ]
        paint updated
        { state with Colors = updated } |> refreshActiveColor, Cmd.none

    | Remove color ->
        let updated = List.where (fun c -> c.id <> color.id) colors
        paint updated
        { state with Colors = updated }, Cmd.none

    | SetActive code ->
        let active = newColor code colors
        { state with ActiveColor = active }, Cmd.none

    | DrawProfile ->
        paint colors
        state, Cmd.none

let colorDisplays { Colors = colors } (dispatch: Msg -> unit): ReactElement =
    let colorDisplay color =
        let { id = id; name = name; code = code } = color
        [ Html.button
            [ attr.className "color-remove"
              attr.onClick (fun _ -> (Remove >> dispatch) color) ]
          Html.div name
          Html.div
              [ attr.className "color-show"
                attr.style [ style.backgroundColor code ] ] ]

    Html.div
        [ attr.className "colors-list"
          attr.children (List.collect colorDisplay colors) ]

let mixButtons { ActiveColor = active } (dispatch: Msg -> unit): ReactElement =
    Html.div
        [ attr.className "color-interactive"
          attr.children
              [ Html.button
                  [ attr.text "Add"
                    attr.onClick (fun _ -> (Add >> dispatch) active) ]
                Html.label
                    [ attr.for' "chooser"
                      attr.text "Choose" ]
                Html.input
                    [ attr.type' "color"
                      attr.id "chooser"
                      attr.className "color-picker"
                      attr.onTextChange (SetActive >> dispatch)
                      attr.valueOrDefault active.code ] ] ]

let render (state) (dispatch: Msg -> unit) =
    let { Colors = colors; ActiveColor = active } = state

    let blockColors =
        (List.map (fun c -> c.code) colors)
        |> List.toArray
        |> Colors

    let swatch = OnSwatchHover(fun color event -> Browser.Dom.console.log ("Color ", color, " hovered via ", event))
    let change = OnChange(fun event -> Browser.Dom.console.log ("Change via : ", event))
    Html.div
        [ attr.children
            [ colorDisplays state dispatch
              mixButtons state dispatch ] ]



open Elmish.HMR

Program.mkProgram init update render
|> Program.withReactBatched "elmish-app"
|> Program.run
