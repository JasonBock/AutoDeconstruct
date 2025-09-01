* For `new string[] { nameof(MyType.Id), "Age" }`
  * For a `nameof(MyType.Age)`
    * `(((attributeCreationOperation.Arguments[2].Value as IArrayCreationOperation).Initializer as IArrayInitializerOperation).ElementValues[0] as INameOfOperation).ConstantValue.Value`
  * For a `"Age"`
    * `(((attributeCreationOperation.Arguments[2].Value as IArrayCreationOperation).Initializer as IArrayInitializerOperation).ElementValues[1] as ILiteralOperation).ConstantValue.Value`
  * For a `"A" + "ge"`
    * `(((attributeCreationOperation.Arguments[2].Value as IArrayCreationOperation).Initializer as IArrayInitializerOperation).ElementValues[1] as IBinaryOperation).ConstantValue.Value`
* For `[nameof(MyType.Age, "Age")]`
  * For a `nameof(MyType.Age)`
    * `(((attributeCreationOperation.Arguments[2].Value as IConversionOperation).Operand as ICollectionExpressionOperation).Elements[0] as INameOfOperation).ConstantValue.Value`
  * For a `"Age"`
    * `(((attributeCreationOperation.Arguments[2].Value as IConversionOperation).Operand as ICollectionExpressionOperation).Elements[1] as ILiteralOperation).ConstantValue.Value`
  * For a `"A" + "ge"`
    * `(((attributeCreationOperation.Arguments[2].Value as IConversionOperation).Operand as ICollectionExpressionOperation).Elements[1] as IBinaryOperation).ConstantValue.Value`

* DONE - Get rid of `SearchForExtensionMethods` and related implementation. Let the compiler tell the user if there is a conflict, which is what this switch is trying to do. There's no point in having the generator do that work.
* DONE - Add tests for attribute
* DONE - Pass data through model and use it during code gen
* Add filtering tests
    * DONE - Unit
        * DONE - Auto
        * DONE - TargetAuto
    * DONE - Analyzer
        * DONE - Auto
        * DONE - TargetAuto
    * Integration
        * Auto
        * TargetAuto
* What happens if you add the attribute more than once with different filtering characteristics?
* What happens if you put a property name in the filter list, and that property by name does not exist on the target type?