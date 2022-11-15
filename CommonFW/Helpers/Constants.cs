namespace Demo.CommonFramework.Helpers
{
    public static class Constants
    {

        //JsonHelper Constants
        public const string HASH_NOT_NULL = "#notnull";
        public const string HASH_NULL = "#isnull";
        public const string HASH_IS_DATE = "#isdate";
        public const string HASH_IS_INTEGER = "#isinteger";
        public const string HASH_IS_INTEGER_GREAT_ZERO = "#isintegergreaterthanzero";
        public const string HASH_IS_INTEGER_LOW_ZERO = "#isintegerlowerthanzero";
        public const string HASH_NOT_EMPTY_ARRAY = "#notemptyarray";
        public const string HASH_NOT_PRESENT = "#notpresent";
        public const string HASH_IS_DECIMAL = "#isdecimal";
        public const string HASH_IS_NUMBER_GREAT_ZERO = "#isnumbergreaterthanzero";

        //BaseUtils
        public static char StepParameterSeparatorChar => ';';
        public static char StepVariablesIndicatorChar => '$';
        public static char StepHashVariablesIndicatorChar => '#';
        public static char StepTildeVariablesIndicatorChar => '~';

        //Origin.Framework.Base.OriginPage
        public const string ANY = @"#any";
    }
}
