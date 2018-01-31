// This file is used by Code Analysis to maintain SuppressMessage 
// attributes that are applied to this project. 
// Project-level suppressions either have no target or are given 
// a specific target and scoped to a namespace, type, member, etc. 
//
// To add a suppression to this file, right-click the message in the 
// Error List, point to "Suppress Message(s)", and click 
// "In Project Suppression File". 
// You do not need to add suppressions to this file manually. 

//This warning is due to the namespace naming. We are following naming convention as suggested by KALE. 
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "Kale.Framework.Common.DataAccessLayer")]

//*********
//This warning with "CA1004:GenericMethodsShouldProvideTypeParameter"
//is suppressed as this is reported as bug from FXCop. 
//Please refer http://social.msdn.microsoft.com/Forums/en-US/vstscode/thread/81f356fd-3821-47bd-ad30-c0a6b70aade5 for more details
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Scope = "member", Target = "Kale.Framework.Common.DataAccessLayer.DataAccessObject.#GetSPOutParameterValue`1(System.String)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Scope = "member", Target = "Kale.Framework.Common.DataAccessLayer.DataAccessObject.#RetrieveScalar`1(System.String,System.String,System.Collections.ObjectModel.Collection`1<Kale.Framework.Common.DataAccessLayer.StoreProcedureParameters>)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Scope = "member", Target = "Kale.Framework.Common.DataAccessLayer.DBOperations.#ExecuteReader`1()")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Scope = "member", Target = "Kale.Framework.Common.DataAccessLayer.DBOperations.#ExecuteScalar`1()")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Scope = "member", Target = "Kale.Framework.Common.DataAccessLayer.DBOperations.#GetSPOutParameterValue`1(System.String)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Scope = "member", Target = "Kale.Framework.Common.DataAccessLayer.IDBOperations.#ExecuteReader`1()")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Scope = "member", Target = "Kale.Framework.Common.DataAccessLayer.IDBOperations.#ExecuteScalar`1()")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Scope = "member", Target = "Kale.Framework.Common.DataAccessLayer.DataAccessObject.#RetrieveDataReader`1(System.String,System.String,System.Collections.ObjectModel.Collection`1<Kale.Framework.Common.DataAccessLayer.StoreProcedureParameters>)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Scope = "member", Target = "Kale.Framework.Common.DataAccessLayer.DataAccessObject.#RetrieveScalar`1(System.String,System.String,System.Collections.ObjectModel.Collection`1<Kale.Framework.Common.DataAccessLayer.StoreProcedureParameter>)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Scope = "member", Target = "Kale.Framework.Common.DataAccessLayer.DataAccessObject.#RetrieveDataReader`1(System.String,System.String,System.Collections.ObjectModel.Collection`1<Kale.Framework.Common.DataAccessLayer.StoreProcedureParameter>)")]
//*********