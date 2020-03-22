module Colors

type Color = {
    name: string
    code: string
    id: int
}

let toColor id (name, code) =
    { name = name; code = code; id = id}

let getCodes colors =
    List.map (fun {code = code} -> code) colors

let activismColors : list<Color> = List.mapi toColor [
  ("Kashmir Red", "#DB0401")
  ("Sudan Blue", "#1B628E") 
  ("Uighur Blue", "#56A1E4") 
  ("Uighur Blue", "#56A1E4")
]

let activismCodes = getCodes activismColors
let distinctCodes = getCodes >> List.distinct >> List.length
let nextId source : int = (List.maxBy (fun x -> x.id) source).id + 1