module Colors

type Color = {
    name: string
    hex: string
}

let toColor (name, hex) =
    { name = name; hex = hex }

let getHexes colors =
    Seq.map (fun color -> color.hex) colors

let activismColors : list<Color> = List.map toColor [
  ("Kashmir Red", "#DB0401")
  ("Sudan Blue", "#1B628E") 
  ("Uighur Blue", "#56A1E4") 
  ("Uighur Blue", "#56A1E4")
]

let activismHexes = getHexes activismColors