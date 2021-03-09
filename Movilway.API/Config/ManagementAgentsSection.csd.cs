//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Movilway.API.Config
{
    
    
    /// <summary>
    /// The User Configuration Element.
    /// </summary>
    public partial class User : global::System.Configuration.ConfigurationElement
    {
        
        #region IsReadOnly override
        /// <summary>
        /// Gets a value indicating whether the element is read-only.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "2.0.0.0")]
        public override bool IsReadOnly()
        {
            return false;
        }
        #endregion
        
        #region Role Property
        /// <summary>
        /// The XML name of the <see cref="Role"/> property.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "2.0.0.0")]
        internal const string RolePropertyName = "role";
        
        /// <summary>
        /// Gets or sets the Role.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "2.0.0.0")]
        [global::System.ComponentModel.DescriptionAttribute("The Role.")]
        [global::System.Configuration.ConfigurationPropertyAttribute(global::Movilway.API.Config.User.RolePropertyName, IsRequired=true, IsKey=true, IsDefaultCollection=false)]
        public string Role
        {
            get
            {
                return ((string)(base[global::Movilway.API.Config.User.RolePropertyName]));
            }
            set
            {
                base[global::Movilway.API.Config.User.RolePropertyName] = value;
            }
        }
        #endregion
        
        #region Agent Property
        /// <summary>
        /// The XML name of the <see cref="Agent"/> property.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "2.0.0.0")]
        internal const string AgentPropertyName = "agent";
        
        /// <summary>
        /// Gets or sets the Agent.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "2.0.0.0")]
        [global::System.ComponentModel.DescriptionAttribute("The Agent.")]
        [global::System.Configuration.ConfigurationPropertyAttribute(global::Movilway.API.Config.User.AgentPropertyName, IsRequired=true, IsKey=false, IsDefaultCollection=false)]
        public string Agent
        {
            get
            {
                return ((string)(base[global::Movilway.API.Config.User.AgentPropertyName]));
            }
            set
            {
                base[global::Movilway.API.Config.User.AgentPropertyName] = value;
            }
        }
        #endregion
        
        #region Password Property
        /// <summary>
        /// The XML name of the <see cref="Password"/> property.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "2.0.0.0")]
        internal const string PasswordPropertyName = "password";
        
        /// <summary>
        /// Gets or sets the Password.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "2.0.0.0")]
        [global::System.ComponentModel.DescriptionAttribute("The Password.")]
        [global::System.Configuration.ConfigurationPropertyAttribute(global::Movilway.API.Config.User.PasswordPropertyName, IsRequired=true, IsKey=false, IsDefaultCollection=false)]
        public string Password
        {
            get
            {
                return ((string)(base[global::Movilway.API.Config.User.PasswordPropertyName]));
            }
            set
            {
                base[global::Movilway.API.Config.User.PasswordPropertyName] = value;
            }
        }
        #endregion
    }
}
namespace Movilway.API.Config
{
    
    
    /// <summary>
    /// The ApiConfiguration Configuration Section.
    /// </summary>
    public partial class ApiConfiguration : global::System.Configuration.ConfigurationSection
    {
        
        #region Singleton Instance
        /// <summary>
        /// The XML name of the ApiConfiguration Configuration Section.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "2.0.0.0")]
        internal const string ApiConfigurationSectionName = "Movilway.API.Config";
        
        /// <summary>
        /// Gets the ApiConfiguration instance.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "2.0.0.0")]
        public static global::Movilway.API.Config.ApiConfiguration Instance
        {
            get
            {
                return ((global::Movilway.API.Config.ApiConfiguration)(global::System.Configuration.ConfigurationManager.GetSection(global::Movilway.API.Config.ApiConfiguration.ApiConfigurationSectionName)));
            }
        }
        #endregion
        
        #region Xmlns Property
        /// <summary>
        /// The XML name of the <see cref="Xmlns"/> property.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "2.0.0.0")]
        internal const string XmlnsPropertyName = "xmlns";
        
        /// <summary>
        /// Gets the XML namespace of this Configuration Section.
        /// </summary>
        /// <remarks>
        /// This property makes sure that if the configuration file contains the XML namespace,
        /// the parser doesn't throw an exception because it encounters the unknown "xmlns" attribute.
        /// </remarks>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "2.0.0.0")]
        [global::System.Configuration.ConfigurationPropertyAttribute(global::Movilway.API.Config.ApiConfiguration.XmlnsPropertyName, IsRequired=false, IsKey=false, IsDefaultCollection=false)]
        public string Xmlns
        {
            get
            {
                return ((string)(base[global::Movilway.API.Config.ApiConfiguration.XmlnsPropertyName]));
            }
        }
        #endregion
        
        #region IsReadOnly override
        /// <summary>
        /// Gets a value indicating whether the element is read-only.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "2.0.0.0")]
        public override bool IsReadOnly()
        {
            return false;
        }
        #endregion
        
        #region ManagementUsers Property
        /// <summary>
        /// The XML name of the <see cref="ManagementUsers"/> property.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "2.0.0.0")]
        internal const string ManagementUsersPropertyName = "managementUsers";
        
        /// <summary>
        /// Gets or sets the ManagementUsers.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "2.0.0.0")]
        [global::System.ComponentModel.DescriptionAttribute("The ManagementUsers.")]
        [global::System.Configuration.ConfigurationPropertyAttribute(global::Movilway.API.Config.ApiConfiguration.ManagementUsersPropertyName, IsRequired=false, IsKey=false, IsDefaultCollection=false)]
        public global::Movilway.API.Config.Users ManagementUsers
        {
            get
            {
                return ((global::Movilway.API.Config.Users)(base[global::Movilway.API.Config.ApiConfiguration.ManagementUsersPropertyName]));
            }
            set
            {
                base[global::Movilway.API.Config.ApiConfiguration.ManagementUsersPropertyName] = value;
            }
        }
        #endregion
        
        #region IBankEntities Property
        /// <summary>
        /// The XML name of the <see cref="IBankEntities"/> property.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "2.0.0.0")]
        internal const string IBankEntitiesPropertyName = "iBankEntities";
        
        /// <summary>
        /// Gets or sets the IBankEntities.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "2.0.0.0")]
        [global::System.ComponentModel.DescriptionAttribute("The IBankEntities.")]
        [global::System.Configuration.ConfigurationPropertyAttribute(global::Movilway.API.Config.ApiConfiguration.IBankEntitiesPropertyName, IsRequired=false, IsKey=false, IsDefaultCollection=false)]
        public global::Movilway.API.Config.IBankEntities IBankEntities
        {
            get
            {
                return ((global::Movilway.API.Config.IBankEntities)(base[global::Movilway.API.Config.ApiConfiguration.IBankEntitiesPropertyName]));
            }
            set
            {
                base[global::Movilway.API.Config.ApiConfiguration.IBankEntitiesPropertyName] = value;
            }
        }
        #endregion
    }
}
namespace Movilway.API.Config
{
    
    
    /// <summary>
    /// A collection of User instances.
    /// </summary>
    [global::System.Configuration.ConfigurationCollectionAttribute(typeof(global::Movilway.API.Config.User), CollectionType=global::System.Configuration.ConfigurationElementCollectionType.BasicMapAlternate, AddItemName=global::Movilway.API.Config.Users.UserPropertyName)]
    public partial class Users : global::System.Configuration.ConfigurationElementCollection
    {
        
        #region Constants
        /// <summary>
        /// The XML name of the individual <see cref="global::Movilway.API.Config.User"/> instances in this collection.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "2.0.0.0")]
        internal const string UserPropertyName = "user";
        #endregion
        
        #region Overrides
        /// <summary>
        /// Gets the type of the <see cref="global::System.Configuration.ConfigurationElementCollection"/>.
        /// </summary>
        /// <returns>The <see cref="global::System.Configuration.ConfigurationElementCollectionType"/> of this collection.</returns>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "2.0.0.0")]
        public override global::System.Configuration.ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return global::System.Configuration.ConfigurationElementCollectionType.BasicMapAlternate;
            }
        }
        
        /// <summary>
        /// Gets the name used to identify this collection of elements
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "2.0.0.0")]
        protected override string ElementName
        {
            get
            {
                return global::Movilway.API.Config.Users.UserPropertyName;
            }
        }
        
        /// <summary>
        /// Indicates whether the specified <see cref="global::System.Configuration.ConfigurationElement"/> exists in the <see cref="global::System.Configuration.ConfigurationElementCollection"/>.
        /// </summary>
        /// <param name="elementName">The name of the element to verify.</param>
        /// <returns>
        /// <see langword="true"/> if the element exists in the collection; otherwise, <see langword="false"/>.
        /// </returns>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "2.0.0.0")]
        protected override bool IsElementName(string elementName)
        {
            return (elementName == global::Movilway.API.Config.Users.UserPropertyName);
        }
        
        /// <summary>
        /// Gets the element key for the specified configuration element.
        /// </summary>
        /// <param name="element">The <see cref="global::System.Configuration.ConfigurationElement"/> to return the key for.</param>
        /// <returns>
        /// An <see cref="object"/> that acts as the key for the specified <see cref="global::System.Configuration.ConfigurationElement"/>.
        /// </returns>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "2.0.0.0")]
        protected override object GetElementKey(global::System.Configuration.ConfigurationElement element)
        {
            return ((global::Movilway.API.Config.User)(element)).Role;
        }
        
        /// <summary>
        /// Creates a new <see cref="global::Movilway.API.Config.User"/>.
        /// </summary>
        /// <returns>
        /// A new <see cref="global::Movilway.API.Config.User"/>.
        /// </returns>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "2.0.0.0")]
        protected override global::System.Configuration.ConfigurationElement CreateNewElement()
        {
            return new global::Movilway.API.Config.User();
        }
        #endregion
        
        #region Indexer
        /// <summary>
        /// Gets the <see cref="global::Movilway.API.Config.User"/> at the specified index.
        /// </summary>
        /// <param name="index">The index of the <see cref="global::Movilway.API.Config.User"/> to retrieve.</param>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "2.0.0.0")]
        public global::Movilway.API.Config.User this[int index]
        {
            get
            {
                return ((global::Movilway.API.Config.User)(base.BaseGet(index)));
            }
        }
        
        /// <summary>
        /// Gets the <see cref="global::Movilway.API.Config.User"/> with the specified key.
        /// </summary>
        /// <param name="role">The key of the <see cref="global::Movilway.API.Config.User"/> to retrieve.</param>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "2.0.0.0")]
        public global::Movilway.API.Config.User this[object role]
        {
            get
            {
                return ((global::Movilway.API.Config.User)(base.BaseGet(role)));
            }
        }
        #endregion
        
        #region Add
        /// <summary>
        /// Adds the specified <see cref="global::Movilway.API.Config.User"/> to the <see cref="global::System.Configuration.ConfigurationElementCollection"/>.
        /// </summary>
        /// <param name="user">The <see cref="global::Movilway.API.Config.User"/> to add.</param>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "2.0.0.0")]
        public void Add(global::Movilway.API.Config.User user)
        {
            base.BaseAdd(user);
        }
        #endregion
        
        #region Remove
        /// <summary>
        /// Removes the specified <see cref="global::Movilway.API.Config.User"/> from the <see cref="global::System.Configuration.ConfigurationElementCollection"/>.
        /// </summary>
        /// <param name="user">The <see cref="global::Movilway.API.Config.User"/> to remove.</param>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "2.0.0.0")]
        public void Remove(global::Movilway.API.Config.User user)
        {
            base.BaseRemove(this.GetElementKey(user));
        }
        #endregion
        
        #region GetItem
        /// <summary>
        /// Gets the <see cref="global::Movilway.API.Config.User"/> at the specified index.
        /// </summary>
        /// <param name="index">The index of the <see cref="global::Movilway.API.Config.User"/> to retrieve.</param>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "2.0.0.0")]
        public global::Movilway.API.Config.User GetItemAt(int index)
        {
            return ((global::Movilway.API.Config.User)(base.BaseGet(index)));
        }
        
        /// <summary>
        /// Gets the <see cref="global::Movilway.API.Config.User"/> with the specified key.
        /// </summary>
        /// <param name="role">The key of the <see cref="global::Movilway.API.Config.User"/> to retrieve.</param>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "2.0.0.0")]
        public global::Movilway.API.Config.User GetItemByKey(string role)
        {
            return ((global::Movilway.API.Config.User)(base.BaseGet(((object)(role)))));
        }
        #endregion
        
        #region IsReadOnly override
        /// <summary>
        /// Gets a value indicating whether the element is read-only.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "2.0.0.0")]
        public override bool IsReadOnly()
        {
            return false;
        }
        #endregion
    }
}
namespace Movilway.API.Config
{
    
    
    /// <summary>
    /// The IBankEntity Configuration Element.
    /// </summary>
    public partial class IBankEntity : global::System.Configuration.ConfigurationElement
    {
        
        #region IsReadOnly override
        /// <summary>
        /// Gets a value indicating whether the element is read-only.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "2.0.0.0")]
        public override bool IsReadOnly()
        {
            return false;
        }
        #endregion
        
        #region Name Property
        /// <summary>
        /// The XML name of the <see cref="Name"/> property.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "2.0.0.0")]
        internal const string NamePropertyName = "name";
        
        /// <summary>
        /// Gets or sets the Name.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "2.0.0.0")]
        [global::System.ComponentModel.DescriptionAttribute("The Name.")]
        [global::System.Configuration.ConfigurationPropertyAttribute(global::Movilway.API.Config.IBankEntity.NamePropertyName, IsRequired=true, IsKey=true, IsDefaultCollection=false)]
        public string Name
        {
            get
            {
                return ((string)(base[global::Movilway.API.Config.IBankEntity.NamePropertyName]));
            }
            set
            {
                base[global::Movilway.API.Config.IBankEntity.NamePropertyName] = value;
            }
        }
        #endregion
        
        #region Agent Property
        /// <summary>
        /// The XML name of the <see cref="Agent"/> property.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "2.0.0.0")]
        internal const string AgentPropertyName = "agent";
        
        /// <summary>
        /// Gets or sets the Agent.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "2.0.0.0")]
        [global::System.ComponentModel.DescriptionAttribute("The Agent.")]
        [global::System.Configuration.ConfigurationPropertyAttribute(global::Movilway.API.Config.IBankEntity.AgentPropertyName, IsRequired=true, IsKey=false, IsDefaultCollection=false)]
        public string Agent
        {
            get
            {
                return ((string)(base[global::Movilway.API.Config.IBankEntity.AgentPropertyName]));
            }
            set
            {
                base[global::Movilway.API.Config.IBankEntity.AgentPropertyName] = value;
            }
        }
        #endregion
        
        #region URL Property
        /// <summary>
        /// The XML name of the <see cref="URL"/> property.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "2.0.0.0")]
        internal const string URLPropertyName = "url";
        
        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "2.0.0.0")]
        [global::System.ComponentModel.DescriptionAttribute("The URL.")]
        [global::System.Configuration.ConfigurationPropertyAttribute(global::Movilway.API.Config.IBankEntity.URLPropertyName, IsRequired=true, IsKey=false, IsDefaultCollection=false)]
        public string URL
        {
            get
            {
                return ((string)(base[global::Movilway.API.Config.IBankEntity.URLPropertyName]));
            }
            set
            {
                base[global::Movilway.API.Config.IBankEntity.URLPropertyName] = value;
            }
        }
        #endregion
    }
}
namespace Movilway.API.Config
{
    
    
    /// <summary>
    /// A collection of IBankEntity instances.
    /// </summary>
    [global::System.Configuration.ConfigurationCollectionAttribute(typeof(global::Movilway.API.Config.IBankEntity), CollectionType=global::System.Configuration.ConfigurationElementCollectionType.BasicMapAlternate, AddItemName=global::Movilway.API.Config.IBankEntities.IBankEntityPropertyName)]
    public partial class IBankEntities : global::System.Configuration.ConfigurationElementCollection
    {
        
        #region Constants
        /// <summary>
        /// The XML name of the individual <see cref="global::Movilway.API.Config.IBankEntity"/> instances in this collection.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "2.0.0.0")]
        internal const string IBankEntityPropertyName = "iBankEntity";
        #endregion
        
        #region Overrides
        /// <summary>
        /// Gets the type of the <see cref="global::System.Configuration.ConfigurationElementCollection"/>.
        /// </summary>
        /// <returns>The <see cref="global::System.Configuration.ConfigurationElementCollectionType"/> of this collection.</returns>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "2.0.0.0")]
        public override global::System.Configuration.ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return global::System.Configuration.ConfigurationElementCollectionType.BasicMapAlternate;
            }
        }
        
        /// <summary>
        /// Gets the name used to identify this collection of elements
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "2.0.0.0")]
        protected override string ElementName
        {
            get
            {
                return global::Movilway.API.Config.IBankEntities.IBankEntityPropertyName;
            }
        }
        
        /// <summary>
        /// Indicates whether the specified <see cref="global::System.Configuration.ConfigurationElement"/> exists in the <see cref="global::System.Configuration.ConfigurationElementCollection"/>.
        /// </summary>
        /// <param name="elementName">The name of the element to verify.</param>
        /// <returns>
        /// <see langword="true"/> if the element exists in the collection; otherwise, <see langword="false"/>.
        /// </returns>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "2.0.0.0")]
        protected override bool IsElementName(string elementName)
        {
            return (elementName == global::Movilway.API.Config.IBankEntities.IBankEntityPropertyName);
        }
        
        /// <summary>
        /// Gets the element key for the specified configuration element.
        /// </summary>
        /// <param name="element">The <see cref="global::System.Configuration.ConfigurationElement"/> to return the key for.</param>
        /// <returns>
        /// An <see cref="object"/> that acts as the key for the specified <see cref="global::System.Configuration.ConfigurationElement"/>.
        /// </returns>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "2.0.0.0")]
        protected override object GetElementKey(global::System.Configuration.ConfigurationElement element)
        {
            return ((global::Movilway.API.Config.IBankEntity)(element)).Name;
        }
        
        /// <summary>
        /// Creates a new <see cref="global::Movilway.API.Config.IBankEntity"/>.
        /// </summary>
        /// <returns>
        /// A new <see cref="global::Movilway.API.Config.IBankEntity"/>.
        /// </returns>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "2.0.0.0")]
        protected override global::System.Configuration.ConfigurationElement CreateNewElement()
        {
            return new global::Movilway.API.Config.IBankEntity();
        }
        #endregion
        
        #region Indexer
        /// <summary>
        /// Gets the <see cref="global::Movilway.API.Config.IBankEntity"/> at the specified index.
        /// </summary>
        /// <param name="index">The index of the <see cref="global::Movilway.API.Config.IBankEntity"/> to retrieve.</param>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "2.0.0.0")]
        public global::Movilway.API.Config.IBankEntity this[int index]
        {
            get
            {
                return ((global::Movilway.API.Config.IBankEntity)(base.BaseGet(index)));
            }
        }
        
        /// <summary>
        /// Gets the <see cref="global::Movilway.API.Config.IBankEntity"/> with the specified key.
        /// </summary>
        /// <param name="name">The key of the <see cref="global::Movilway.API.Config.IBankEntity"/> to retrieve.</param>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "2.0.0.0")]
        public global::Movilway.API.Config.IBankEntity this[object name]
        {
            get
            {
                return ((global::Movilway.API.Config.IBankEntity)(base.BaseGet(name)));
            }
        }
        #endregion
        
        #region Add
        /// <summary>
        /// Adds the specified <see cref="global::Movilway.API.Config.IBankEntity"/> to the <see cref="global::System.Configuration.ConfigurationElementCollection"/>.
        /// </summary>
        /// <param name="iBankEntity">The <see cref="global::Movilway.API.Config.IBankEntity"/> to add.</param>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "2.0.0.0")]
        public void Add(global::Movilway.API.Config.IBankEntity iBankEntity)
        {
            base.BaseAdd(iBankEntity);
        }
        #endregion
        
        #region Remove
        /// <summary>
        /// Removes the specified <see cref="global::Movilway.API.Config.IBankEntity"/> from the <see cref="global::System.Configuration.ConfigurationElementCollection"/>.
        /// </summary>
        /// <param name="iBankEntity">The <see cref="global::Movilway.API.Config.IBankEntity"/> to remove.</param>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "2.0.0.0")]
        public void Remove(global::Movilway.API.Config.IBankEntity iBankEntity)
        {
            base.BaseRemove(this.GetElementKey(iBankEntity));
        }
        #endregion
        
        #region GetItem
        /// <summary>
        /// Gets the <see cref="global::Movilway.API.Config.IBankEntity"/> at the specified index.
        /// </summary>
        /// <param name="index">The index of the <see cref="global::Movilway.API.Config.IBankEntity"/> to retrieve.</param>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "2.0.0.0")]
        public global::Movilway.API.Config.IBankEntity GetItemAt(int index)
        {
            return ((global::Movilway.API.Config.IBankEntity)(base.BaseGet(index)));
        }
        
        /// <summary>
        /// Gets the <see cref="global::Movilway.API.Config.IBankEntity"/> with the specified key.
        /// </summary>
        /// <param name="name">The key of the <see cref="global::Movilway.API.Config.IBankEntity"/> to retrieve.</param>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "2.0.0.0")]
        public global::Movilway.API.Config.IBankEntity GetItemByKey(string name)
        {
            return ((global::Movilway.API.Config.IBankEntity)(base.BaseGet(((object)(name)))));
        }
        #endregion
        
        #region IsReadOnly override
        /// <summary>
        /// Gets a value indicating whether the element is read-only.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "2.0.0.0")]
        public override bool IsReadOnly()
        {
            return false;
        }
        #endregion
    }
}
