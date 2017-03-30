using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace testUseDll.Complex
{
    [FlagsAttribute]
    [Serializable]    
    public enum NameEntityType
    {
        PersonName,
        PlaceName,
        OrganizationName 
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    [Serializable]
    [DataContract]
    public class NameEntity
    {
        public NameEntity()
        {

        }
        public string _name
        {
            get;
            set;
        }
        public NameEntityType _type
        {
            get;
            set;
        }
        public int _highlightLength
        {
            get;
            set;
        }
        public double _score
        {
            get;
            set;
        }
    }

    public abstract class SafeNativeHandle : SafeHandle
    {
        public SafeNativeHandle() : base(IntPtr.Zero, true) { }

        public override bool IsInvalid
        {
            get { return IsClosed || handle == IntPtr.Zero; }
        }
    }
}
