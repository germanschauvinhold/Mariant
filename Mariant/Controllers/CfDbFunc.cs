using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace WebAsistida.lib
{
    //++ After/Before
    //se utiliza para after/before upsert/delete con parametros
    public delegate String CfDbQryTxtParamFunc(JObject param);
    // se utiliza para un proceso after/before upsert/delete
    public delegate List<String> CfDbQryTxtProcessParamFunc(JObject param);
    //-- After/Before


    //++ Carga masiva
    public delegate String CfStageTableDbQryTxtFunc(); // validaciones Import Staging
    public delegate String CfStageImportDbQryTxtFunc(JObject param); // After Fastload Actions
    public delegate String CfAfterStageChckQryTxtFunc(); // NO 2015-04-16
    public delegate String CfDbImportQryTxtFunc(); //Before Import Upsert/Delete
    public delegate List<String> CfDbQryTxtImportProcessParamFunc(out String[] paramNames); // After Import Upsert
    public delegate String CfDbQryTextValidateProcessFunc(String currentTime); // After Import Upsert/Delete
    public delegate String CfDbQryTextPreTransmissionFunc(); // After Import Transmission
    public delegate String CfDbQryTxtImportTransmissionFunc(String RepEvent_id, String RepUser_id, String MaxRow_id); // para queries de transmision
    //++ RepDailySheet ++ ++ ForcedRequest ++
    public delegate String CfDbQryTxtImportProcessFunc(String RepEvent_id, String RepSession_id, String RepUser_id); // After/Before Import RepDailySheet
    //-- RepDailySheet -- -- ForcedRequest --
    //++ ForcedRequest ++
    public delegate String CfDbQryTxtImportFrcdRqstFunc(String RepEvent_id, String RepSession_id, String RepUser_id, String MaxRow_id); //Only Forced Request
    //-- ForcedRequest --
    //-- Carga masiva
}