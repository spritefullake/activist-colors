module Colors

type Color = {
    name: string
    hex: string
    id: int
}

let toColor id (name, hex) =
    { name = name; hex = hex; id = id}

let getHexes colors =
    List.map (fun color -> color.hex) colors

let activismColors : list<Color> = List.mapi toColor [
  ("Kashmir Red", "#DB0401")
  ("Sudan Blue", "#1B628E") 
  ("Uighur Blue", "#56A1E4") 
  ("Uighur Blue", "#56A1E4")
]

let activismHexes = getHexes activismColors