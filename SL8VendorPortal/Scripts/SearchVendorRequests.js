var anOpen = [];
var oVRequestTable;
var asInitVals = new Array();
var intMaxRecordCount = 100;


TableTools.BUTTONS.download = {
    "sAction": "text",
    "sTag": "default",
    "sFieldBoundary": "",
    "sFieldSeperator": "\t",
    "sNewLine": "<br>",
    "sToolTip": "",
    "sButtonClass": "DTTT_button_text",
    "sButtonClassHover": "DTTT_button_text_hover",
    "sButtonText": "Download",
    "mColumns": "all",
    "bHeader": true,
    "bFooter": true,
    "sDiv": "",
    "fnMouseover": null,
    "fnMouseout": null,
    "fnClick": function (nButton, oConfig) {
        var oParams = this.s.dt.oApi._fnAjaxParameters(this.s.dt);
        var iframe = document.createElement('iframe');
        iframe.style.height = "0px";
        iframe.style.width = "0px";
        oParams.push({ "name": "MaxRecordCount", "value": intMaxRecordCount });
        iframe.src = oConfig.sUrl + "?" + $.param(oParams);
        document.body.appendChild(iframe);
    },
    "fnSelect": null,
    "fnComplete": null,
    "fnInit": null
};

function FormatDate(objDate, blnIncludeTime) {
    var objDateProcessed = new Date(parseInt(objDate.replace("/Date(", "").replace(")/", ""), 10));

    //return objDateProcessed.getMonth() + 1 + "/" + objDateProcessed.getDate() + "/" + objDateProcessed.getFullYear();
    //return dateFormat(objDateProcessed, "dddd, mmmm dS, yyyy, h:MM:ss TT"); //returns Thursday, November 1st, 2012, 12:00:00 AM
    if (blnIncludeTime)
        return dateFormat(objDateProcessed, "mm-dd-yyyy HH:MM:ss"); //returns 11-01-2012 12:30:00
    else
        return dateFormat(objDateProcessed, "mm-dd-yyyy"); //returns  11-01-2012
}

$(document).ready(function () {
    var objAllWarehouses = AllWarehouses();
    var oTimerId;


    $.editable.addInputType('autogrow', {   //adds the autogrow plugin for editing notes
        element: function (settings, original) {
            var textarea = $('<textarea />');
            if (settings.rows) {
                textarea.attr('rows', settings.rows);
            } else {
                textarea.height(settings.height);
            }
            $(this).append(textarea);
            return (textarea);
        },
        plugin: function (settings, original) {
            $('textarea', this).autogrow(settings.autogrow);
        }
    });

    oVRequestTable = $('#objItems').dataTable({
        "bProcessing": true,
        "bServerSide": true,
        "bJQueryUI": true,
        "oTableTools": {
            "aButtons": [
                {
                    "sExtends": "download",
                    "sButtonText": "Excel Download",
                    "sUrl": sPrintVendorRequestsUrl //+ "?MaxRecordCount=" + intMaxRecordCount // "/generate_csv.php"
                }
            ]
        },
        "sPaginationType": "full_numbers",
        "sScrollX": "100%", //puts scrollbars around the datatable
        "sDom": 'T<"clear">Rlrtip', //The 'R' enables column reorder with resize; UPDATE took out the f from "Rlfrtip" to hide the search textbox
        //"bStateSave": true, //saves the state of datatables, when the end user reloads or revisits a page its previous state is retained// Unfortuantely was holding the values of my submitted forms and causing my queries to fail...
        "oColReorder": {
            "iFixedColumns": 5,
            "fnReorderCallback": function () {
                //will execute the javascript in here each time the columns are reordered....
            },
            "aiOrder": [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18] //This puts the specific columns in this order; I left here for reference since I don't actually change the order of the columns
                                                                                          //It must match the number of columns that you have or else you'll recieve a ''oDTSettings' is undefined' error...
        },
        "sAjaxSource": document.URL,
        "sServerMethod": "POST",
        "fnServerData": function (sSource, aoData, fnCallback, oSettings) {
            /*This function gets fired every time a keydown event is fired from one of the search boxes. The issue I was having was that an exception "ValueFactory attempted to access the Value property of this instance." kept getting thrown when 
            I was debugging and viewed this page. Turns out that the reason was because every keystroke was issuing a database call and when I had the multicolumn filtering that had columns of type 'checkbox', these checkboxes would each fire a db lookup
            while they were loading. The client and server couldn't keep up with those rapid calls to the database and populating the jQueryDataTablesModelBinder fast enough and so the exception was thrown. The end result was that I needed to pause the calls
            to the database until the user was done typing. I tried implementing fnSetFilteringDelay http://datatables.net/plug-ins/api#fnSetFilteringDelay but the problem was that it wouuld then only populate the sSearch variable and not the sSearch_0, sSearch_1, etc.
            that multicolumn filtering utilized. I could populate sSearch_0, sSearch_1, etc. by using fnFilter(strValue, intCol) where strValue was the value I wanted placed and intCol was the integer of the column, but I still had no way of pulling the values that 
            were to be submitted since fnFilter essentially cleared all the values from the posted form (or something anyway because I could not figure out how to access them...) Finally after spending a day of searching for a solution, I stumbled upon this one
            which simply uses the javascript setTimeout function to delay the execution of the post for 1 second (1000 milliseconds). I store the timer ID in a variable so that anytime a keystroke occurs befor the 1 second is up, the timer is cancelled and the previous db
            lookup never executes; a new one instead is initialized. This function successfully eliminated countless unnecessary calls to the database.
            UPDATE - I needed to add a parameter to the server call that would act as an insurance against the intial call to Vendor requests where every item in the collection is populated until filtering begins. I discovered that the best way to do this was to add a
            parameter to aoData which is the data structure that is populated with all the information which gets populated into the jQueryDataTablesModelBinder object on the server (via the MVC linker). I chose to not modify jQueryDataTablesModelBinder since the MaxRecordCount
            is really only used by the controller.*/

            window.clearTimeout(oTimerId); //clear the timer if it still exists
            oTimerId = window.setTimeout(function () {
                aoData.push({ "name": "MaxRecordCount", "value": intMaxRecordCount }); //adds the MaxRecordCount data to the array sent to the server...

                oSettings.jqXHR = $.ajax({  //This is the setting that does the posting of the data...
                    "dataType": 'json',
                    "type": "POST",
                    "url": sSource,
                    "data": aoData,
                    "success": fnCallback
                });
            }, 1000) //wait 1000 milliseconds (1 sec) before executing the function...
        },
        "oLanguage": { "sSearch": "Search Order #s:" },
        "aoColumns": [
            {   //I had to add this column to the table otherwise it was passing the button href to my update Url; Datatables wants the first column to be a primary key...
                //When mDataProp is null, I can update, when it is set to ID I can search on it...
                "mDataProp": null, //When posting, there was no value being passed to the 'id' parameter that datatables datatables automatically posts; THE ONLY WAY that I could get a the key value (column 'ID' in my database) to populate to the id parameter
                "sDefaultContent": "ID",    //Was to set mDataProp to null, set sDefaultContent to some arbitrary value (I used 'ID') and then populate the cell with the 'ID' property of the row which maps to my ID column in my database..
                "bVisible": true, //Setting this column as hidden removed it from the display and negated the need to include it in my makeEditable aoColumns array, however I still had to add the column to GetVRTableHTML()
                "fnRender": function (oObj) {
                    return oObj.aData["ID"]; //accessing the ID column of the specific row
                }
            },
            {
                "mDataProp": null, //Note that I had a problem with this column being first because when the datatable loads, it automatically sorts based on the first column; since this column had a null value
                "bSortable": false, //it would pass that null value to the data call. I actually fixed this by modifying the code in InMemoryRepositories.cs so that if there was an error, it just returns the list
                "sClass": "control center",
                "sDefaultContent": '<img src="' + sOpenImageUrl + '">'
            },
            {/*I had a really tough time with this one because I needed to have my ID column first in order to have the ID passed as an update parameter and when I added this parameter below I kept getting the error 
                "Unable to get property 'style' of undefined or null reference". Turns out that I needed to add another th in Search.cshtml thead & tfoot and then another {} in .makeEditable so that my search boxes
                would line up properly*/
                "mDataProp": "ID"
            },
            { "mDataProp": "RequestCategoryCode" },
            { "mDataProp": "OrderNo" },
            { "mDataProp": "Item" },
            { "mDataProp": "Processed" },
            { "mDataProp": "LineNo" },
            { "mDataProp": "ReleaseNo" },
            { "mDataProp": "SourceWarehouse" },
            { "mDataProp": "DestWarehouse" },
            { "mDataProp": "Qty" },
            { "mDataProp": "QtyLoss" },
            {
                "mDataProp": "DateProcessed",
                "fnRender": function (oObj) {
                    if (oObj.aData.DateProcessed != null && oObj.aData.DateProcessed !== undefined) {
                        return FormatDate(oObj.aData.DateProcessed, true);
                    }
                }
            },
            {
                "mDataProp": "DateRequested",
                "fnRender": function (oObj) {
                    if (oObj.aData.DateRequested != null && oObj.aData.DateRequested !== undefined) {
                        return FormatDate(oObj.aData.DateRequested, true);
                    }
                }
            },
            {
                "mDataProp": "DateUpdated",
                "fnRender": function (oObj) {
                    if (oObj.aData.DateUpdated != null && oObj.aData.DateUpdated !== undefined) {
                        return FormatDate(oObj.aData.DateUpdated, true);
                    }
                }
            },
            { "mDataProp": "Approved" },
            { "mDataProp": "Creator" },
            { "mDataProp": "Updater"}]
    }).columnFilter({
        //sPlaceHolder: "head:after",// this will change the location of the search columns to the header instead of the footer.
        aoColumns: [
        null,
        null,
        {
            sSelector: "#IDFilter"
        },
        {
            sSelector: "#CategoryCodeFilter",
            type: "checkbox", //type: "select", //I changed this to use a checkbox list instead of a select list..
            values: RequestCategories()
        },
        {
            sSelector: "#OrderNoFilter"
        },
        {
            sSelector: "#ItemFilter"
        },
        {
            sSelector: "#ProcessedFilter",
            type: "checkbox",
            values: ["True", "False"],
            tablo_checkboxes: ["False"]
        },
        {},
        {},
        {
            sSelector: "#SourceWhseFilter",
            type: "checkbox",
            values: objAllWarehouses
        },
        {
            sSelector: "#DestWhseFilter",
            type: "checkbox",
            values: objAllWarehouses
        },
        {
            type: "number-range"
        },
        {
            type: "number-range"
        },
        {
            sSelector: "#DateProcessedFilter",
            type: "date-range"
        },
        {
            sSelector: "#DateRequestedFilter",
            type: "date-range"
        },
        {
            sSelector: "#DateUpdatedFilter",
            type: "date-range"
        },
        {},
        {}, //creator
        {} //updater
    ]
    }).makeEditable({ //I kept getting the error "Object doesn't support property or method 'live'" until I changed line 1260 of jquery.dataTables.editable.js 'live' method to 'on'
        sUpdateURL: sUpdateVRUrl,
        //sAddURL: sAddVRUrl,
        //sDeleteURL: sDeleteVRUrl,
        aoColumns: [
            null,
            null,
            null,
            null,
            null,
            null,
            {
                type: 'select',
                onblur: 'submit',  //values can be "ignore", "submit", or "cancel",
                indicator: 'Saving...',
                data: "{'true':'Process', 'false':'UnProcess'}"
                //oUpdateParameters: {
                //    ID: function () {
                //        return 1;
                //    }
                //}
                //submit: 'Save changes'//shows a submit button with the text "Save Changes"
                //id: 'ID'  //gives the label 'id' the label 'PostedNoteData'
                //submitdata: { id: 1, value: "dood" } //sumbits the following 'parametername: value' pairs to the controller.
            },
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null
            ]
    });




    //        $('#objItems tbody').on('click', 'td.control', function () { //specific td item is clicked
    //        $('#objItems tbody').on('click', 'td', function () {          //any td item clicked
    //        $('#objItems tbody').on('click', 'td.control', function () {  //anytime  row is clicked
    $('#objItems tbody').on('click', 'td.control', function () {
        var nTr;
        var i;
        var rowIndex;
        var nDetailsRow;
        var sLineTableName;
        var objRow;


        nTr = this.parentNode;
        i = $.inArray(nTr, anOpen);

        rowIndex = oVRequestTable.fnGetPosition(nTr); //get the index of the current row
        sLineTableName = 'dtProcessTable' + rowIndex;

        objRow = oVRequestTable.fnGetData(rowIndex);

        if (i === -1) { //the datatable is opening the row...
            $('img', this).attr('src', sCloseImageUrl);
            nDetailsRow = oVRequestTable.fnOpen(nTr, GetVRNoteDetailHTML(objRow), 'details');
            $('div.innerDetails', nDetailsRow).slideDown();
            anOpen.push(nTr);


            $('.EditRequestNotes').editable(sVRNotesUpdateUrl, { //make the note detail editable...
                id: 'PostedNoteData',  //gives the label 'id' the label 'PostedNoteData'
                name: 'NoteContent', //labels the updated data as NoteContent instead of 'value'
                submitdata: { ID: objRow.ID, notes: objRow.notes },
                type: "autogrow", //specifies to use the autogrow input specified above
                submit: 'OK', //Adds the OK button that submits post
                cancel: 'Cancel', //adds the Cancel button that cancels the edit
                tooltip: "Click to edit.",
                onblur: "ignore", //what happens when user clicks out of textarea values can be "ignore", "submit", or "cancel"
                indicator: '<img src="' + sProgressImageUrl + '">', //This is what shows while ajax is processing. Could also just be a text message like 'Saving...'
                data: function (value, settings) { //This gets fired when the control goes into edit mode
                    /* Convert <br> to newline. */
                    var retval = value.replace(/<br[\s\/]?>/gi, '\n');
                    return retval;
                },
                callback: function (value, settings) {//This is fired after the control uses ajax to save the data to the server and the JSON Result is recieved back from the server
                    sValue = JSON.parse(value); //puts the JSON data into objects form so they can be accessed like sValue.LastUpdate, etc.

                    if (sValue.Success) { }
                    else {
                        alert('The edits failed to save');
                    }
                    $('#VRNoteEditorDiv').html(sValue.HTMLNotes); //Sets the NoteEditorDiv to the html of the newly saved Notes Object's HTML property
                    //window.location.reload();
                }
            });
        }
        else {
            $('img', this).attr('src', sOpenImageUrl);
            $('div.innerRowDetails', $(nTr).next()[0]).slideUp(function () {
                oVRequestTable.fnClose(nTr);
                anOpen.splice(i, 1);
            });
        }
    });

    //This function gets all the Transfer Order Request Categories from the Database
    function RequestCategories() {
        $.ajaxSetup({ async: false }); //my call had to be synchronouse here or else the datatable would render before the selectlist was returned...
        //I modified the below so that I didn't have to specify a different controller action for CO, PO, and TO request categories. I pass the RequestCategory ID in a query string so the controllerselects the correct one
        $.post(sRequestCatUrl + '?&RequestCategoryID=*&ReturnAll=True', {}, function (data) {
            RequestCategories = FormatCheckBoxColumnJSON(data); //FormatSelectColumnJSON(data);//Changed this so that my implementation used a checkboxlist instead of a selectlist
        });
        $.ajaxSetup({ async: true });
        return RequestCategories;
    }

    //When I set this up as async, then I couldn't call it simultaneously in my columnFilter() instantiation, when it is synchronous the database doesn't return the data in time for rendering it on the browser.
    //So I made a global variable objAllWarehouses and populate it with the values of this function when the document.ready() function loads.
    function AllWarehouses() {
        $.ajaxSetup({ async: false }); //This will set the asynchronous call to synchronous so that a value isn't returned until the ajax call recieves a response
        $.post(sAllWhsesUrl, {}, function (data) {
            AllWarehouses = FormatCheckBoxColumnJSON(data); //Wants data like this: ["JPC", "KMC", "PPI"];
        });
        $.ajaxSetup({ async: true }); //Sets ajax back up to synchronous

        return AllWarehouses;
    }

    function FormatCheckBoxColumnJSON(x) {
        var finalEdit = new Array();

        //Loop through the list
        for (i = 0; i < x.length; i++) {
            //alert(x[i][0]);
            finalEdit[i] = x[i][0]; //The list is actually a 2 dimensional string array; I just take the first string
        }

        return finalEdit;
    }

    function FormatSelectColumnJSON(x) {
        var orig = x;
        var stgify = JSON.stringify(orig);
        var splitchar = ['\",\"', '\"],[\"', '[', ']', '\"'];
        var joinchar = ['\':\'', '\', \'', '', '', '\''];

        //This is the loop that does the replacement of the above characters
        for (i = 0; i < 5; i++) {
            stgify = stgify.split(splitchar[i]);
            tmp = stgify.join(joinchar[i]);
            stgify = tmp;
        }

        stgify = stgify.split(','); //splits the comma separated records into an array 
        for (i = 0; i < stgify.length; i++) {
            stgify[i] = stgify[i].split(':')[0]; //at this point I had records looking like 'CORequest':'CO Request' so I split on the : and only kept the first element
            stgify[i] = stgify[i].split('\'').join(''); //removed the single quotes from the record
        }

        var finalEdit = stgify;

        return finalEdit;
    }
});