﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Movilway.API.Service.ExtendedApi.Provider.Payment
{
    public static class SoapRequest
    {
        public static string INICIAR_PAGO = "<soapenv:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:web=\"https://puntodepago.plataformadepago.com/secure/webservices\"> <soapenv:Header/> <soapenv:Body> <web:Iniciar_Pago soapenv:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\"> <usuario xsi:type=\"xsd:string\">{0}</usuario> <clave xsi:type=\"xsd:string\">{1}</clave> <llavemd5 xsi:type=\"xsd:string\">{2}</llavemd5> <referencia xsi:type=\"xsd:string\">{3}</referencia> <total_con_iva xsi:type=\"xsd:string\">{4}</total_con_iva> <valor_iva xsi:type=\"xsd:string\">{5}</valor_iva> <descripcion_pago xsi:type=\"xsd:string\">{6}</descripcion_pago> <email xsi:type=\"xsd:string\">{7}</email> <Id_cliente xsi:type=\"xsd:string\">{8}</Id_cliente> <nombre_cliente xsi:type=\"xsd:string\">{9}</nombre_cliente> <apellido_cliente xsi:type=\"xsd:string\">{10}</apellido_cliente> <telefono_cliente xsi:type=\"xsd:string\">{11}</telefono_cliente> <direccion xsi:type=\"xsd:string\">{12}</direccion> <pais xsi:type=\"xsd:string\">{13}</pais> <ciudad xsi:type=\"xsd:string\">{14}</ciudad> <ip xsi:type=\"xsd:string\">{15}</ip> <firma xsi:type=\"xsd:string\" xsi:nil=\"true\"></firma> <info_opcional1 xsi:type=\"xsd:string\">{16}</info_opcional1> <info_opcional2 xsi:type=\"xsd:string\">{17}</info_opcional2> <info_opcional3 xsi:type=\"xsd:string\">{18}</info_opcional3> <lista_codigos_servicio_multicredito xsi:type=\"xsd:string\" xsi:nil=\"true\"></lista_codigos_servicio_multicredito> <lista_nit_codigos_servicio_multicredito xsi:type=\"xsd:string\" xsi:nil=\"true\"></lista_nit_codigos_servicio_multicredito> <lista_valores_con_iva xsi:type=\"xsd:string\" xsi:nil=\"true\"></lista_valores_con_iva> <lista_valores_iva xsi:type=\"xsd:string\" xsi:nil=\"true\"></lista_valores_iva> <total_codigos_servicio xsi:type=\"xsd:string\" xsi:nil=\"true\"></total_codigos_servicio> <fotografiaurl xsi:type=\"xsd:string\" xsi:nil=\"true\"></fotografiaurl> <grabacionurl xsi:type=\"xsd:string\" xsi:nil=\"true\"></grabacionurl> <llamadaconfirmacionnumero xsi:type=\"xsd:string\" xsi:nil=\"true\"></llamadaconfirmacionnumero> <facebook xsi:type=\"xsd:string\" xsi:nil=\"true\"></facebook> <urlretorno xsi:type=\"xsd:string\">{19}</urlretorno> <CodigoDelBanco xsi:type=\"xsd:string\">{20}</CodigoDelBanco> <TipoDeUsuario xsi:type=\"xsd:string\">{21}</TipoDeUsuario> <latitud xsi:type=\"xsd:string\" xsi:nil=\"true\"></latitud> <longitud xsi:type=\"xsd:string\" xsi:nil=\"true\"></longitud> <exactitud xsi:type=\"xsd:string\" xsi:nil=\"true\"></exactitud> </web:Iniciar_Pago> </soapenv:Body> </soapenv:Envelope>";
    }
}