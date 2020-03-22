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
    | SetActiveHex of string

let nextId (source : Colors.Color list) : int = 
    (List.maxBy (fun x -> x.id) source).id + 1

let distinctCodes colors = 
    List.length << List.distinct << getHexes <| colors


let init() =
    { Colors = activismColors |> Seq.toList
      ActiveColor =
          { name = sprintf "Color %d" (distinctCodes activismColors)
            hex = "#aaaeee"
            id = nextId activismColors } }, Cmd.none

let dropFirst predicate items =
    let rec loop validated items =
        match items with
        | [] -> []
        | x :: xs ->
            if predicate x then loop (validated @ [ x ]) xs else validated @ xs
    loop [] items

let newColor hex colors =
    let picker color =
        if color.hex = hex then
            Some color
        else
            None
    match List.tryPick picker colors with
    | Some color ->
        {color with id = nextId colors}
    | None ->
        {
            name = sprintf "Color %d" (distinctCodes colors)
            hex = hex
            id = nextId colors
        }

let runMixer (hexes : string seq) =
    let origin = (200., 200.)
    let angles = [ 0.0 .. Math.PI / 2. .. Math.PI * 2. ]
    let zipped = Seq.zip angles hexes   

    for angle, color in zipped do
        rectangularCut 100. 100.
        |> transform (rotate angle)
        //|> Seq.iter (fun x -> ctx.stroke())
        |> movementTrack (Continuous origin)
        |> Seq.map unwrap
        |> Shape.fill ctx !^color

let update (msg: Msg) (state: State): State * Cmd<Msg> =
    printfn "THE COLORS ARE %A" state.Colors
    match msg with
    | Add color -> 
        { state with Colors = state.Colors @ [ color ] }, 
        Cmd.ofMsg (SetActiveHex color.hex) //Refresh the ID of the active color
        //this ^ is an abuse of the Elmish state model; should refactor

    | Remove color -> 
        { state with Colors = List.where (fun c -> c.id <> color.id) state.Colors },
        Cmd.none

    | SetActiveHex hex ->          
        let active = newColor hex state.Colors
        { state with
              ActiveColor = active },
        Cmd.none              

let colorDisplays (state : State) (dispatch : Msg -> unit) : ReactElement list =
        let colorDisplay color =
            let {id = id; name = name; hex = hex} = color
            [ Html.button
                [ attr.className "color-remove"
                  attr.onClick (fun _ -> (Remove >> dispatch) color) ]
              Html.p name
              Html.div
                  [ attr.className "color-show"
                    attr.style [ style.backgroundColor hex ] ] ]
        List.collect colorDisplay state.Colors

let render (state) (dispatch: Msg -> unit) =
    let { Colors = colors; ActiveColor = active } = state

    let blockColors =
        (List.map (fun c -> c.hex) colors)
        |> List.toArray
        |> Colors

    let swatch = OnSwatchHover(fun color event -> Browser.Dom.console.log ("Color ", color, " hovered via ", event))
    let change = OnChange(fun event -> Browser.Dom.console.log ("Change via : ", event))
    Html.div
        [ attr.className "color-interactive"
          attr.children
              [ Html.h1 "Colors List"
                Html.div
                    [ attr.className "colors-list"
                      attr.children (colorDisplays state dispatch) ]
                BlockPicker [ blockColors; swatch; change ]

                Html.h2 "Add a Color!"
                Html.button
                    [ attr.text "Add Color"
                      attr.onClick (fun _ -> (Add >> dispatch) active) ]
                Html.input
                    [ attr.type' "color"
                      attr.className "color-picker"
                      attr.onTextChange (SetActiveHex >> dispatch)
                      attr.valueOrDefault active.hex ] ] ]
    

open Elmish.HMR

Program.mkProgram init update render
|> Program.withReactBatched "elmish-app"
|> Program.run






// write Fable
ctx.textAlign <- "center"
ctx.fillText ("What is this", gridWidth * 0.5, gridWidth * 0.5)

printfn "done!"
printfn "w: %f" w
