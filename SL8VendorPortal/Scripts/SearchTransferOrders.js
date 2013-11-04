var anOpen = [];
var oTable;
//var selectedParentRowIndex;I was using this when I was getting the OrderNo and LineNo variables from the parent datatable into my frmAddVendorRequest


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
        iframe.src = oConfig.sUrl + "?" + $.param(oParams);
        document.body.appendChild(iframe);
    },
    "fnSelect": null,
    "fnComplete": null,
    "fnInit": null
};

//I implemented this function which uses my custom DateTimeFormatter.js dateFormat() function
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
    var oTimerId;


    $.editable.addInputType('autogrow', {   //adds the autogrow plugin for editing notes
        element: function (settings, original) {
            var textarea = $('<textarea />');
            if (settings.rows) {
                textarea.attr('rows', settings.rows);
            } else {
                textarea.height(settings.height);
            }

            if (settings.cols) {
                //textarea.attr('cols', settings.cols);// commenting these out fixed the horizontal sizing issue I was having with the textarea
            } else {
                //textarea.width(settings.width);
            }
            $(this).append(textarea);
            return (textarea);
        },
        plugin: function (settings, original) {
            $('textarea', this).autogrow(settings.autogrow);
        }
    });

    //note that these statements have nothing to do with autogrow in my datatables. I put the below here so that I could get autogrow to work on my frmAddVendorRequest; autogrow for datatables is handled in the .MakeEditable instantiation
    //$('textarea').autogrow(); // will apply autogrow to all textareas...
    $('.MyAutogrow').autogrow(); //applies autogrow to any textarea with statement class="MyAutogrow"...


    oTable = $('#objItems').dataTable({
        "bProcessing": true,
        "bServerSide": true,
        "sDom": 'T<"clear">Rlfrtip', //Enables column reorder with resize. 'T<"clear"> adds the 'download' button
        "oTableTools": {
            "aButtons": [
                {
                    "sExtends": "download",
                    "sButtonText": "Excel Download",
                    "sUrl": sPrintTOUrl // "/generate_csv.php"
                }
            ]
        },
        "sScrollX": "100%", //creates a scrollbar for the wide table
        "bJQueryUI": true, //enables themeroller for datatables...http://datatables.net/examples/basic_init/themes.html 
		"sPaginationType": "full_numbers",
        "sAjaxSource": document.URL,
        "sServerMethod": "POST",
        "fnServerData": function (sSource, aoData, fnCallback, oSettings) {
        
            window.clearTimeout(oTimerId);
            oTimerId = window.setTimeout(function () {
                oSettings.jqXHR = $.ajax({
                    "dataType": 'json',
                    "type": "POST",
                    "url": sSource,
                    "data": aoData,
                    "success": fnCallback
                });
            }, 1000)
        },
        "oLanguage": { "sSearch": "Search Transfer #s:" },
        "bAutoWidth": false, // Disable the auto width calculation 
        "aoColumns": [
            {
                "mDataProp": null, //Note that I had a problem with this column being first because when the datatable loads, it automatically sorts based on the first column; since this column had a null value
                "bSortable": false, //it would pass that null value to the data call. I actually fixed this by modifying the code in InMemoryRepositories.cs so that if there was an error, it just returns the list
                "sClass": "control center",
                "sDefaultContent": '<img src="' + sOpenImageUrl + '">',
                "sWidth": "10px" //works in conjuction with bAutoWidth;note that it could also be "30%"
            },
            { "mDataProp": "trn_num" },
            { "mDataProp": "from_whse" },
            { "mDataProp": "to_whse" },
            { 
                "mDataProp": "order_date", 
                "fnRender": function (oObj) {
                    if (oObj.aData.order_date != null && oObj.aData.order_date !== undefined) {
                        return FormatDate(oObj.aData.order_date);
                    }
                }
            },
            { 
                "mDataProp": "CreateDate",
                "fnRender": function (oObj) {
                    if (oObj.aData.CreateDate != null && oObj.aData.CreateDate !== undefined) {
                        return FormatDate(oObj.aData.CreateDate);
                    }
                } 
            },
            { "mDataProp": "weight" },
            { "mDataProp": "qty_packages" },
            { "mDataProp": "CreatedBy" },
            { "mDataProp": "UpdatedBy"}],
        "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
            /*I had been using this method to format the date, however when I implemented the column reordering and resizing plugin, this functionality broke because it referenced
                positions in the datatable. I replaced this functionality with the global function FormatDate and the fnRender function of each column*/
            //if (aData.order_date != null && aData.order_date !== undefined) {
            //    var objorderdate = new Date(parseInt(aData.order_date.replace("/Date(", "").replace(")/", ""), 10));
            //    $('td:eq(4)', nRow).html(objorderdate.getMonth() + 1 + "/" + objorderdate.getDate() + "/" + objorderdate.getFullYear());
            //}

            //if (aData.CreateDate != null && aData.CreateDate !== undefined) {
            //    var objCreateDate = new Date(parseInt(aData.CreateDate.replace("/Date(", "").replace(")/", ""), 10));
            //    $('td:eq(5)', nRow).html(objCreateDate.getMonth() + 1 + "/" + objCreateDate.getDate() + "/" + objCreateDate.getFullYear());
            //}
        }
    });


    $('#objItems tbody').on('click', 'td.control', function () {
        var nTr;
        var i;
        var rowIndex;
        var nDetailsRow;
        var sLineTableName;
        var sLineURL;
        var sOrderNo;
        var tInnerTable

        nTr = this.parentNode;
        i = $.inArray(nTr, anOpen);

        rowIndex = oTable.fnGetPosition(nTr); //get the index of the current row
        //selectedParentRowIndex = rowIndex; //I added this here so that I can pass the index number to my VendorRequest Creating Function***
        sLineTableName = 'dtLineTable' + rowIndex;

        sOrderNo = oTable.fnGetData(rowIndex).trn_num; //get the order number of the current row

        sLineURL = document.URL.replace(/Search$/, ""); //uses a RegEx to remove the string 'Search' only if it is at the end of the URL 
        sLineURL = sLineURL + 'SearchTOLinesByOrder';

        if (i === -1) { //the datatable is opening the row...
            $('img', this).attr('src', sCloseImageUrl);
            nDetailsRow = oTable.fnOpen(nTr, GetLineTableHTML(oTable, nTr, sLineTableName), 'details');
            $('div.innerDetails', nDetailsRow).slideDown();
            anOpen.push(nTr);


            tInnerTable = $('#' + sLineTableName).dataTable({ //when referencing the table for it's api, it always wants to be prefaced with #
                "bProcessing": true,
                "sDom": "Rlfrtip", //Enables column reorder with resize
                "bFilter": false,   //hides the search box
                "bPaginate": false, //disables paging functionality
                "bServerSide": true,
                "sAjaxSource": sLineURL + '?&OrderNo=' + sOrderNo, //pass the order number to the orderline url as a querystring; note query string variables are delimited by &
                "sDom": '<"top">rt<"bottom"flp><"clear">', //hides the filter box and the 'showing recordno of records' message
                "sServerMethod": "POST",
                "aoColumns": [
                        { //I can't have a bound column followed by a null column;This column used to be "mDataProp": "trn_num", but then when i added my additional column below then I kept getting errors
                            fnRender: makeViewNotesBtn, //My solution was to make both columns null by omitting the "mDataProp" attribute (could also be accomplished with '"mDataProp" : null')
                            "bSortable": false,
                            "bSearchable": false,
                            "sDefaultContent": '<img src="' + sOpenImageUrl + '">'
                        },
                        {  //I can't have the same mDataProp used twice on the datatable (like "trn_num" above), and when I used null I kept getting a 'Requested unknown parameter '0' datatables error
                            fnRender: makeVendorRequestBtn,
                            "bSortable": false,
                            "bSearchable": false,
                            "sDefaultContent": '<img src="' + sOpenImageUrl + '">'//adding sDefaultContent solved the error from having a null dataprop
                        },
                        { "mDataProp": "trn_line" },
                        { "mDataProp": "item" },
                        { "mDataProp": "qty_req" },
                        { "mDataProp": "qty_shipped" },
                        { "mDataProp": "qty_received" },
                        { "mDataProp": "qty_loss" },
                        { 
                            "mDataProp": "sch_rcv_date",
                            "fnRender": function (oObj) {
                                if (oObj.aData.sch_rcv_date != null && oObj.aData.sch_rcv_date !== undefined) {
                                    return FormatDate(oObj.aData.sch_rcv_date);
                                }
                            } 
                        },
                        { 
                            "mDataProp": "sch_ship_date",
                            "fnRender": function (oObj) {
                                if (oObj.aData.sch_ship_date != null && oObj.aData.sch_ship_date !== undefined) {
                                    return FormatDate(oObj.aData.sch_ship_date);
                                }
                            } 
                        },
                        { "mDataProp": "CreatedBy" },
                        { "mDataProp": "UpdatedBy" },
                        { 
                            "mDataProp": "CreateDate",
                            "fnRender": function (oObj) {
                                if (oObj.aData.CreateDate != null && oObj.aData.CreateDate !== undefined) {
                                    return FormatDate(oObj.aData.CreateDate);
                                }
                            }
                        },
                        { 
                            "mDataProp": "RecordDate",
                            "fnRender": function (oObj) {
                                if (oObj.aData.RecordDate != null && oObj.aData.RecordDate !== undefined) {
                                    return FormatDate(oObj.aData.RecordDate);
                                }
                            }
                        }]
            });
        }
        else {
            $('img', this).attr('src', sOpenImageUrl);
            $('div.innerDetails', $(nTr).next()[0]).slideUp(function () {
                oTable.fnClose(nTr);
                anOpen.splice(i, 1);
            });
        }
    });
});

function loadNotesDialog(sUrl) {
    var oNotesTable;

    /*The below sets the html in the div to the proper table with ID objNotes so datatables can instantiate it. Before I did this I had a problem with datatables erroring out because
    I was trying to initialize the same data table a second time*/
    $("#NotesDialogDiv").html(GetNoteTableHTML()); //Just a hidden empty div on Search.cshtml that the popup uses...

    oNotesTable = $('#objNotes').dataTable({
        "sDom": "Rlfrtip", //Enables column reorder with resize
        "bJQueryUI": true,
        "bProcessing": true,
        "bServerSide": true,
        "sAjaxSource": sUrl, //document.URL,
        "sServerMethod": "POST",
        "aoColumns": [
            {
                "mDataProp": null, //Note that I had a problem with this column being first because when the datatable loads, it automatically sorts based on the first column; since this column had a null value
                "bSortable": false, //it would pass that null value to the data call. I actually fixed this by modifying the code in InMemoryRepositories.cs so that if there was an error, it just returns the list
                "sClass": "control center",
                "sDefaultContent": '<img src="' + sOpenImageUrl + '">'
            },
            { "mDataProp": "NoteDesc" },
            { "mDataProp": "CreatedBy" },
            { "mDataProp": "UpdatedBy" },
            { 
                "mDataProp": "LastUpdated"
                //"fnRender": function (oObj) {
                //    if (oObj.aData.LastUpdated != null && oObj.aData.LastUpdated !== undefined) {
                //        return FormatDate(oObj.aData.LastUpdated);
                //    }
                //}
            }],
        "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
            /*This needed to be reinstated because fnRender changed the value that was sent to the controller and broke the note updating functionality due to the time
            being truncated off by the FormatDate function; see NotesController's UpdateNote method for details*/
            if (aData.LastUpdated != null && aData.LastUpdated !== undefined) {
                var objLastUpdated = new Date(parseInt(aData.LastUpdated.replace("/Date(", "").replace(")/", ""), 10));
                $('td:eq(4)', nRow).html(objLastUpdated.getMonth() + 1 + "/" + objLastUpdated.getDate() + "/" + objLastUpdated.getFullYear());
            }
        }
    });

    $('#objNotes tbody').on('click', 'td.control', function () {
        var nTr;
        var i;
        var rowIndex;
        var nDetailsRow;
        var sLineTableName;
        var objRow;

        nTr = this.parentNode;
        i = $.inArray(nTr, anOpen);

        rowIndex = oNotesTable.fnGetPosition(nTr); //get the index of the current row
        sLineTableName = 'dtLineTable' + rowIndex;

        objRow = oNotesTable.fnGetData(rowIndex);

        if (i === -1) { //the datatable is opening the row...
            $('img', this).attr('src', sCloseImageUrl);
            nDetailsRow = oNotesTable.fnOpen(nTr, GetNoteDetailHTML(objRow), 'details');
            $('div.innerRowDetails', nDetailsRow).slideDown();
            anOpen.push(nTr);

            $('.EditNotes').editable(sNotesUpdateUrl, { //make the note detail editable...
                id: 'PostedNoteData',  //gives the label 'id' the label 'PostedNoteData'
                name: 'NoteContent', //labels the updated data as NoteContent instead of 'value'
                submitdata: { SpecificNoteToken: objRow.SpecificNoteToken, NoteDesc: objRow.NoteDesc, LastUpdated: objRow.LastUpdated }, //passes the additional parameters SpecificNoteToken and NoteDesc
                type: "autogrow", //specifies to use the autogrow input specified above
                submit: 'Ok', //Adds the OK button that submits post
                //submit: '<input type="submit" value="Ok" style="position:relative; top: 20%; left:-100px;">', //Adds the OK button that submits post http://datatables.net/usage/options#sDom might work to position buttons correctly...
                cancel: 'Cancel', //adds the Cancel button that cancels the edit
                tooltip: "Click to edit.",
                onblur: "ignore", //what happens when user clicks out of textarea values can be "ignore", "submit", or "cancel"
                indicator: '<img src="' + sProgressImageUrl + '">', //This is what shows while ajax is processing. Could also just be a text message like 'Saving...'
                data: function (value, settings) { //This gets fired when the control goes into edit mode
                    /* Convert <br> to newline. */
                    //alert(value);
                    var retval = value.replace(/<br[\s\/]?>/gi, '\n');
                    return retval;
                },
                callback: function (value, settings) {//This is fired after the control uses ajax to save the data to the server and the JSON Result is recieved back from the server
                    sValue = JSON.parse(value); //puts the JSON data into objects form so they can be accessed like sValue.LastUpdate, etc.

                    if (sValue.Success) {
                        objRow.LastUpdated = sValue.LastUpdate
                        oNotesTable.fnUpdate(objRow, rowIndex); //required to update the datatable with the new LastUpdate Date, else subsequent edits to notes will fail
                        //alert(sValue.LastUpdate);
                    }
                    else {
                        alert('The edits failed to save');
                    }
                    $('#NoteEditorDiv').html(sValue.HTMLNoteContent);//Sets the NoteEditorDiv to the html of the newly saved Notes Object's HTML property
                    //window.location.reload();
                }
            });
        }
        else {
            $('img', this).attr('src', sOpenImageUrl);
            $('div.innerRowDetails', $(nTr).next()[0]).slideUp(function () {
                oNotesTable.fnClose(nTr);
                anOpen.splice(i, 1);
            });
        }
    });

    // Open this Datatable as a modal dialog box.
    $('#objNotes').dialog({
        modal: true,
        resizable: true,
        position: 'top',
        width: 'auto',
        autoResize: true,
        title: 'Transfer Order Notes'
    });
};

function makeViewNotesBtn(oObj) {
    var sOrderNo = oObj.aData.trn_num;
    var sLineNo = oObj.aData.trn_line;
    var sItemID = oObj.aData.item;
    var sHref


    /*The below is what i was using to call my NotesViewer.cshtml from the controller before i implemented the dialog box*/
    //return "<a href='" + sLineURL + '?&OrderNo=' + sOrderNo + '&LineNo=' + sLineNo + "' class='ViewNotes' title='View Notes'><img src='" + sOpenImageUrl + "' height='12' width='12'></a>";

    sHref = sTONotesUrl + '?&OrderNo=' + sOrderNo + '&LineNo=' + sLineNo; //generate the query string
    return "<a href=\"javascript:loadNotesDialog('" + sHref + "')\" class='ViewNotes' title='View Notes'><img src='" + sOpenImageUrl + "' height='10' width='10'></a>";
};

function makeVendorRequestBtn(oObj) {
    var sOrderNo = oObj.aData.trn_num;
    var sLineNo = oObj.aData.trn_line;
    var sItemID = oObj.aData.item;
    var sHref

    sHref = sVendorRequestsUrl + '?&OrderNo=' + sOrderNo + '&LineNo=' + sLineNo + '&ItemID=' + sItemID + '&RequestType=TO'; //generate the query string
    return "<a href=\"javascript:loadVendorRequestDialog('" + sHref + "')\" class='Process' title='Process'><img src='" + sOpenImageUrl + "' height='10' width='10'></a>";
};

function loadVendorRequestDialog(sUrl) {
    var oVRequestTable;

    //This is a cool little chunk of code that I use to get the values of parameters from the querystring sUrl. For sUrl='/VendorRequests/VendorRequests?&OrderNo=T000000267&LineNo=1' I can call it like GET("OrderNo")[0] to get 'T000000267'
    //or I can call it like data = GET("OrderNo","LineNo"); and get data[0]= 'T000000267' and data[1]= '1'.
    function GET() {
        var data = [];
        for (x = 0; x < arguments.length; ++x)
            data.push(sUrl.match(new RegExp("/\?".concat(arguments[x], "=", "([^\n&]*)")))[1])
        return data;
    }

    //This is a way to create a reusable column, so you don't have to redefine the same variables on each column instantiation of makeEditable()
    //note that all options that are available for editable() from jeditable are available here...
    defaultEditable = {
        tooltip: 'Click to edit',
        type: 'select', //was 'textarea' in the example which called my autogrowing textbox...
        onblur: 'submit',
        data: UserWarehouses()
        //submit: 'Save changes',//shows a submit button
        //cancel: 'Cancel'//creates a cancel button
        //fnOnCellUpdated: function (sStatus, sValue, settings) { //This will show a message to the user after the value has been successfully saved to the server
         //   alert("(Cell Callback): Cell is updated with value " + sValue);
        //} 
    }

    /*The below sets the html in the div to the proper table with ID objNotes so datatables can instantiate it. Before I did this I had a problem with datatables erroring out because
    I was trying to initialize the same data table a second time*/
    $("#VendorRequestDialogDiv").html(GetVRTableHTML()); //Just a hidden empty div on Search.cshtml that the popup uses...


    oVRequestTable = $('#objVendorRequest').dataTable({
        "bJQueryUI": true,
        "sDom": "Rlfrtip", //Enables column reorder with resize
        "bProcessing": true,
        "bServerSide": true,
        "bFilter": false,   //hides the search box
        "bPaginate": false, //disables paging functionality
        "sDom": '<"top">rt<"bottom"flp><"clear">', //hides footer that displays 'showing record 1 of 1'...
        "sAjaxSource": sUrl, //document.URL,
        "sServerMethod": "POST",
        "aoColumns": [
            {   //I had to add this column to the table otherwise it was passing the button href to my update Url; Datatables wants the first column to be a primary key...
                "mDataProp": null, //When posting, there was no value being passed to the 'id' parameter that datatables datatables automatically posts; THE ONLY WAY that I could get a the key value (column 'ID' in my database) to populate to the id parameter
                "sDefaultContent": "ID",    //Was to set mDataProp to null, set sDefaultContent to some arbitrary value (I used 'ID') and then populate the cell with the 'ID' property of the row which maps to my ID column in my database..
                "bVisible": false, //Setting this column as hidden removed it from the display and negated the need to include it in my makeEditable aoColumns array, however I still had to add the column to GetVRTableHTML()
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
            { "mDataProp": "RequestCategoryCode" },
            { "mDataProp": "Qty" },
            { "mDataProp": "QtyLoss" },
            { "mDataProp": "SourceWarehouse" },
            { "mDataProp": "DestWarehouse" },
            { "mDataProp": "Processed" },
            {
                "mDataProp": "OrderNo", //I don't show this row on the datatable, but I need it here so that I can populate it on my frmAddVendorRequest
                "bVisible": false
            },
            {
                "mDataProp": "LineNo", //I don't show this row on the datatable, but I need it here so that I can populate it on my frmAddVendorRequest
                "bVisible": false
            },
            {
                "mDataProp": "Notes", //I don't show this row on the datatable, but I need it here so that I can populate it on my frmAddVendorRequest
                "bVisible": false
            },
            {
                "mDataProp": "RequestCategoryID", //I don't show this row on the datatable, but I need it here so that I can populate it on my frmAddVendorRequest
                "bVisible": false
            }],
        "fnDrawCallback": function () {  //can be used to call a function when the datatable is redrawn; I left it here for reference
            if (typeof oVRequestTable == 'undefined') { //checks to see if the table is undefined or null; I left it here for reference
                //fnOpenClose();
            };
        },
        "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
            /*if (aData.DateRequested != null && aData.DateRequested !== undefined) {
            var objDateRequested = new Date(parseInt(aData.DateRequested.replace("/Date(", "").replace(")/", ""), 10));
            $('td:eq(4)', nRow).html(objDateRequested.getMonth() + 1 + "/" + objDateRequested.getDate() + "/" + objDateRequested.getFullYear());
            }*/

        }
    }).makeEditable({ //I kept getting the error "Object doesn't support property or method 'live'" until I changed line 1260 of jquery.dataTables.editable.js 'live' method to 'on'
        sUpdateURL: sUpdateVRUrl,
        sAddURL: sAddVRUrl,
        sDeleteURL: sDeleteVRUrl,
        //fnOnNewRowPosted: function (data) { //This function gets called after the frmAddVendorRequest has been posted to the server, but before the row is added to the datatable.
        //Gets the value of the trn_num of the parent table. I need this to create a vendor request
        //alert(oTable.fnGetData(selectedParentRowIndex).trn_num);


        //if (data.indexOf("Error", 0) == 0) {
        //    //Show error message
        //    return false;
        //} else {
        //    //Show success message and add row
        //    return true;
        //}
        //},
        fnOnAdding: function () {//This event gets fired right before the form is submitted to the server. I can use this time to populate the variables I need to add a new record into the database such as OrderNo, OrderLine, OrderRelease, etc.
            //In order for this to work I needed to add the columns to the datatable and then hide them, put the hiddent textboxes on the frmAddVendorRequest form with the proper rel="#" attribute (where # is the column order
            //That they were added in the column instantiation. I then populate thier values here...
            //$("#trace").append(test = "Adding new record");//was the sample function from the internet; I'm not sure what it does...

            //To use the below I need to have the row index set when the parent datatable is opening to show the lines...
            //document.getElementById('OrderNo').value = oTable.fnGetData(selectedParentRowIndex).trn_num; 
            //document.getElementById('LineNo').value = oTable.fnGetData(selectedParentRowIndex).trn_num;

            //A much better method that uses the GET function defined above and retrieves the values from the querystring...
            var dataValues = GET("OrderNo", "LineNo", "ItemID");
            document.getElementById('OrderNo').value = dataValues[0];
            document.getElementById('LineNo').value = dataValues[1];
            document.getElementById('Item').value = dataValues[2];
            document.getElementById('RequestCategoryID').value = 2;//Transfer Orders have a request category of 2

            return true;
        },
        sAddNewRowFormId: "frmAddVendorRequest", //specifies the ID of the form that will be used to add a new row...
        "oAddNewRowFormOptions": { //These options are the same/taken from the jquery.dialog() options http://api.jqueryui.com/dialog/
            "title": "Add new Vendor Request", //This can also be done by setting the attribute 'title = "Add new Vendor Request"' in the form tag
            "height": 450,
            "width": 700
        },
        //sAddNewRowOkButtonId: "btnAddNewCompanyOk",  //The plugin should automatically create these buttons, but I couldn't get them to fire so I added them here and put the corresponding inputs on the form with the respective id's; but it still didnt' work...
        //sAddNewRowCancelButtonId: "btnAddNewCompanyCancel", //It turns out that the problem was that I didn't have my jquery.validate script loaded! Once I added the script then everything worked as expected and the buttons that were automatically created fired correctly.
        aoColumns: [
            null,
            {
                type: 'select',
                onblur: 'submit',  //values can be "ignore", "submit", or "cancel"
                //data: "{'TOReciept':'TOReciept', 'TOShipment':'TOShipment'}",
                data: RequestCategories()
                //submit: 'Save changes'//shows a submit button with the text "Save Changes"
                //id: 'ID'  //gives the label 'id' the label 'PostedNoteData'
                //submitdata: { id: 1, value: "dood" } //sumbits the following 'parametername: value' pairs to the controller.
            },
            {},
            {},
            $.extend({}, defaultEditable, { indicator: 'Saving...' }), //create a column using defaultEditable value that is listed above, also passes an 'indicator' parameter that is specific to this instance...
            {//This is the normal way (as opposed to using above 'defaultEditable' method) of adding columns; note that all options that are available for editable() from jeditable are available here...
            tooltip: 'Click to edit',
            type: 'select',
            onblur: 'submit',
            data: AllWarehouses()
            //submit: 'Save changes',//shows a submit button
            //cancel: 'Cancel'//creates a cancel button
            //fnOnCellUpdated: function (sStatus, sValue, settings) { //This will show a message to the user after the value has been successfully saved to the server
            //    alert("(Cell Callback): Cell is updated with value " + sValue);
            //}
        }]
    });

    $('#objVendorRequest tbody').on('click', 'td.control', function () {

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
            $('div.innerRowDetails', nDetailsRow).slideDown();
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
                    //alert(value);
                    var retval = value.replace(/<br[\s\/]?>/gi, '\n');
                    return retval;
                },
                callback: function (value, settings) {//This is fired after the control uses ajax to save the data to the server and the JSON Result is recieved back from the server
                    sValue = JSON.parse(value); //puts the JSON data into objects form so they can be accessed like sValue.LastUpdate, etc.

                    if (sValue.Success) {
                        //alert('The edits saved');
                        //objRow.LastUpdated = sValue.LastUpdate
                        //oVRequestTable.fnUpdate(objRow, rowIndex); //required to update the datatable with the new LastUpdate Date, else subsequent edits to notes will fail
                        //alert(sValue.LastUpdate);
                    }
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

    //This function needed to be in the same Block as the calling function in the oProcessTable.makeEditable() column call, else I would get the error 'SCRIPT5002: Function expected'. Its almost like the 
    //Script file gets detached from the DOM or something and then the column instantiation can't find the AllWarehouses() function... This took me a day to figure out!
    function AllWarehouses() {
        var sValue;
        //$.ajaxSetup({ async: false }); //This will set the asynchronous call to synchronous so that a value isn't returned until the ajax call recieves a response
        $.post(sAllWhsesUrl, {}, function (data) {//data comes as a collection of objects; same as if you used JSON.parse(), 
            //sValue = JSON.stringify(data);          //so you can immediately access an object in the collection like data[1]. JSON.stringify() puts the collection into a string
            //Note that this $.post is an asynchronous call and so you must return the 'data' variable from the function brackets, else the function will return 'data' before the ajax call has completed.
            AllWarehouses = FormatJSON(data); //I needed to assign the formatted JSON to the function or else the values didn't bind correctly to the HTML; I think it had to do with asychronis nature of the calls
            //alert(data[1]); //The object can be access just like this before JSON.stringify() is called or before it is called in FormatJSON()
            //return sValue;//I was only able to return the actual data values from here until I set $.ajaxSetup({ async: false }); This is due to the asynchronis nature of Ajax
            //return "{'TOReciept':'TOReciept', 'TOShipment':'TOShipment'}";
        });
        //$.ajaxSetup({ async: true });//Sets ajax back up to synchronous
        //return sValue;// returned null until 
        //The below is how the data must be sent to the dropdownlist. I could send this string so I could test that my syntax was correct.
        //return "{'DCE':'', 'JPC':'JAGEMANN PLATING CO', 'KI':'KRUEGER', 'KMC':'KETTLE MORAINE COATINGS', 'MAGC':'Magna Coat', 'MAIN':'Wire Tec Fabricators', 'PPI':'PROFESSIONAL PLATING INC', 'SFAB':'Seal Fab', 'SPLS':'Services PLus'}";
        return AllWarehouses;
    }

    //This function essentially performs the same as AllWarehouses, except that it only returns warehouses that the Vendor has access to so that they cannot specify a source of a location other than one of thier own.
    function UserWarehouses() {
        var sValue;
        $.post(sUserWhsesUrl, {}, function (data) {
            UserWarehouses = FormatJSON(data);
        });
        return UserWarehouses;
    }

    //This function gets all the Transfer Order Request Categories from the Database
    function RequestCategories() {
        var sValue;

        //I modified the below so that I didn't have to specify a different controller action for CO, PO, and TO request categories. I pass the RequestCategory ID in a query string so the controllerselects the correct one
        $.post(sRequestCatUrl + '?&RequestCategoryID=2', {}, function (data) {
            RequestCategories = FormatJSON(data);
        });
        return RequestCategories;
    }

    //The JSON must be in the a format such as "{'TOReciept':'TOReciept', 'TOShipment':'TOShipment'}" This function does that
    function FormatJSON(x) {
        var orig = x;
        var stgify = JSON.stringify(orig);
        var splitchar = ['\",\"', '\"],[\"', '[', ']', '\"'];
        var joinchar = ['\':\'', '\', \'', '', '', '\''];

        //This is the loop that does the replacement of the above characters
        for (i = 0; i < 5; i++) {
            //alert("Split Char: " + splitchar[i] + "\r\n" + "Join Char: " + joinchar[i]);
            stgify = stgify.split(splitchar[i]);
            tmp = stgify.join(joinchar[i]);
            stgify = tmp;
        }
        //The below is the original code; but it didn't format my JSON correctly so I rewrote it. Basically this takes the characters from the splitchar array and replaces them with the 
        //characters specified in the joinchar array
        /*var splitchar = ['\\"', '\',\'', '[', ']', '\"'];
        var joinchar = ['\'', '\':\'', '', '', ''];
        for (i = 0; i < 5; i++) {
        alert("Split Char: " + splitchar[i] + "\r\n" + "Join Char: " + joinchar[i]);
        stgify = stgify.split(splitchar[i]);
        tmp = stgify.join(joinchar[i]);
        stgify = tmp;
        }*/

        stgify = "{" + stgify + "}";
        var finalEdit = stgify;
        return finalEdit;
    }

    // Open this Datatable as a modal dialog box.
    $('#VendorRequestDialogDiv').dialog({
        modal: true,
        resizable: true,
        position: 'top',
        width: 'auto',
        autoResize: true,
        title: 'Transfer Order Requests'
    });
};