module Movement

type Movement<'a> =
    | Continuous of 'a
    | Discontinuous of 'a

let isContinuous = function
  | Continuous _ -> true
  | _ -> false

let map f = function
  | Continuous x -> Continuous (f x)
  | Discontinuous x -> Discontinuous (f x)
let binMap f g = function
  | Continuous x -> Continuous (f x)
  | Discontinuous x -> Discontinuous (g x)
let unwrap = function
  | Continuous x -> x
  | Discontinuous x -> x
let transform f = Seq.map (map f)
let update (f : 'a -> 'b) (item : Movement<'a>) : 'b =
  (map f >> unwrap) item