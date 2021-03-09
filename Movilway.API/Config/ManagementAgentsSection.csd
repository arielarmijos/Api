<?xml version="1.0" encoding="utf-8"?>
<configurationSectionModel xmlns:dm0="http://schemas.microsoft.com/VisualStudio/2008/DslTools/Core" dslVersion="1.0.0.0" Id="4f7527cd-d582-419f-a104-f8c0b13a647f" namespace="Movilway.API.Config" xmlSchemaNamespace="urn:Movilway.API.Config" assemblyName="Movilway.API" xmlns="http://schemas.microsoft.com/dsltools/ConfigurationSectionDesigner">
  <typeDefinitions>
    <externalType name="String" namespace="System" />
    <externalType name="Boolean" namespace="System" />
    <externalType name="Int32" namespace="System" />
    <externalType name="Int64" namespace="System" />
    <externalType name="Single" namespace="System" />
    <externalType name="Double" namespace="System" />
    <externalType name="DateTime" namespace="System" />
    <externalType name="TimeSpan" namespace="System" />
  </typeDefinitions>
  <configurationElements>
    <configurationElement name="User">
      <attributeProperties>
        <attributeProperty name="Role" isRequired="true" isKey="true" isDefaultCollection="false" xmlName="role" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/4f7527cd-d582-419f-a104-f8c0b13a647f/String" />
          </type>
        </attributeProperty>
        <attributeProperty name="Agent" isRequired="true" isKey="false" isDefaultCollection="false" xmlName="agent" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/4f7527cd-d582-419f-a104-f8c0b13a647f/String" />
          </type>
        </attributeProperty>
        <attributeProperty name="Password" isRequired="true" isKey="false" isDefaultCollection="false" xmlName="password" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/4f7527cd-d582-419f-a104-f8c0b13a647f/String" />
          </type>
        </attributeProperty>
      </attributeProperties>
    </configurationElement>
    <configurationSection name="ApiConfiguration" codeGenOptions="Singleton, XmlnsProperty" xmlSectionName="Movilway.API.Config">
      <elementProperties>
        <elementProperty name="ManagementUsers" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="managementUsers" isReadOnly="false">
          <type>
            <configurationElementCollectionMoniker name="/4f7527cd-d582-419f-a104-f8c0b13a647f/Users" />
          </type>
        </elementProperty>
        <elementProperty name="IBankEntities" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="iBankEntities" isReadOnly="false">
          <type>
            <configurationElementCollectionMoniker name="/4f7527cd-d582-419f-a104-f8c0b13a647f/IBankEntities" />
          </type>
        </elementProperty>
      </elementProperties>
    </configurationSection>
    <configurationElementCollection name="Users" xmlItemName="user" codeGenOptions="Indexer, AddMethod, RemoveMethod, GetItemMethods">
      <itemType>
        <configurationElementMoniker name="/4f7527cd-d582-419f-a104-f8c0b13a647f/User" />
      </itemType>
    </configurationElementCollection>
    <configurationElement name="IBankEntity">
      <attributeProperties>
        <attributeProperty name="Name" isRequired="true" isKey="true" isDefaultCollection="false" xmlName="name" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/4f7527cd-d582-419f-a104-f8c0b13a647f/String" />
          </type>
        </attributeProperty>
        <attributeProperty name="Agent" isRequired="true" isKey="false" isDefaultCollection="false" xmlName="agent" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/4f7527cd-d582-419f-a104-f8c0b13a647f/String" />
          </type>
        </attributeProperty>
        <attributeProperty name="URL" isRequired="true" isKey="false" isDefaultCollection="false" xmlName="url" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/4f7527cd-d582-419f-a104-f8c0b13a647f/String" />
          </type>
        </attributeProperty>
      </attributeProperties>
    </configurationElement>
    <configurationElementCollection name="IBankEntities" xmlItemName="iBankEntity" codeGenOptions="Indexer, AddMethod, RemoveMethod, GetItemMethods">
      <itemType>
        <configurationElementMoniker name="/4f7527cd-d582-419f-a104-f8c0b13a647f/IBankEntity" />
      </itemType>
    </configurationElementCollection>
  </configurationElements>
  <propertyValidators>
    <validators />
  </propertyValidators>
</configurationSectionModel>