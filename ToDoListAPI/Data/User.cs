
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


namespace ToDoListAPI.Data
{

using System;
    using System.Collections.Generic;
    
public partial class User
{

    public int ID { get; set; }

    public string Name { get; set; }

    public string Password { get; set; }

    public string EMail { get; set; }

    public string TimeZone { get; set; }

    public Nullable<int> UTCOffset_Hours { get; set; }

    public Nullable<int> UTCOffset_Minutes { get; set; }

}

}
