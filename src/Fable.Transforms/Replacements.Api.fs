module Fable.Transforms.Replacements.Api

#nowarn "1182"

open Fable
open Fable.AST
open Fable.AST.Fable
open Fable.Transforms

type ICompiler = FSharp2Fable.IFableCompiler

let curryExprAtRuntime (com: Compiler) arity (expr: Expr) = Util.curryExprAtRuntime com arity expr

let uncurryExprAtRuntime (com: Compiler) arity (expr: Expr) =
    Util.uncurryExprAtRuntime com arity expr

let partialApplyAtRuntime (com: Compiler) t arity (fn: Expr) (args: Expr list) =
    Util.partialApplyAtRuntime com t arity fn args

let tryField (com: ICompiler) returnTyp ownerTyp fieldName =
    JS.Replacements.tryField com returnTyp ownerTyp fieldName

let tryBaseConstructor (com: ICompiler) ctx (ent: EntityRef) (argTypes: Lazy<Type list>) genArgs args =
    JS.Replacements.tryBaseConstructor com ctx ent argTypes genArgs args

let makeMethodInfo (com: ICompiler) r (name: string) (parameters: (string * Type) list) (returnType: Type) =
    JS.Replacements.makeMethodInfo com r name parameters returnType

let tryType (com: ICompiler) (t: Type) = JS.Replacements.tryType t

let tryCall (com: ICompiler) ctx r t info thisArg args =
    JS.Replacements.tryCall com ctx r t info thisArg args

let error (com: ICompiler) msg = JS.Replacements.error com msg

let defaultof (com: ICompiler) ctx r typ = JS.Replacements.defaultof com ctx r typ

let createMutablePublicValue (com: ICompiler) value = JS.Replacements.createAtom com value

let getRefCell (com: ICompiler) r typ (expr: Expr) =
    JS.Replacements.getRefCell com r typ expr

let setRefCell (com: ICompiler) r (expr: Expr) (value: Expr) =
    JS.Replacements.setRefCell com r expr value

let makeRefCellFromValue (com: ICompiler) r (value: Expr) =
    JS.Replacements.makeRefCellFromValue com r value

let makeRefFromMutableFunc (com: ICompiler) ctx r t (value: Expr) =
    JS.Replacements.makeRefFromMutableFunc com ctx r t value

let makeRefFromMutableValue (com: ICompiler) ctx r t (value: Expr) =
    JS.Replacements.makeRefFromMutableValue com ctx r t value

let makeRefFromMutableField (com: ICompiler) ctx r t (value: Expr) =
    JS.Replacements.makeRefFromMutableField com ctx r t value
