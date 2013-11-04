var anOpen = [];


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

    $('.MyAutogrow').autogrow(); //applies autogrow to any textarea with statement class="MyAutogrow"...

    $('#objItems').dataTable({
        "bProcessing": true,
        "bServerSide": true,
        "sDom": 'T<"clear">Rlfrtip', //Enables column reorder with resize. 'T<"clear"> adds the 'download' button
        "oTableTools": {
            "aButtons": [
                {
                    "sExtends": "download",
                    "sButtonText": "Excel Download",
                    "sUrl": sPrintInventoryUrl // "/generate_csv.php"
                }
            ]
        },
        //"sScrollX": "100%", //Puts scroll bars around the datatable
        "bJQueryUI": true, //puts the box around the column headers
        "sPaginationType": "full_numbers", //enhances the pagination display from simple arrows
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
        "oLanguage": { "sSearch": "Search Item #s:" },
        "aoColumns": [
            {
                fnRender: makeVendorRequestBtn,
                "mDataProp": null,
                "bSortable": false,
                "bSearchable": false,
                "sDefaultContent": '<img src="' + sOpenImageUrl + '">'//adding sDefaultContent solved the error from having a null dataprop
            },
            { "mDataProp": "item", "sWidth": "25%" },
            { "mDataProp": "whse" },
            { "mDataProp": "qty_on_hand" },
            { "mDataProp": "qty_alloc_co" },
            { "mDataProp": "qty_trans" }],
        "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
            //var objOrderdate = new Date(parseInt(aData.order_date.replace("/Date(", "").replace(")/", ""), 10));
            //var objCreateDate = new Date(parseInt(aData.CreateDate.replace("/Date(", "").replace(")/", ""), 10));

            //$('td:eq(3)', nRow).html(objOrderdate.getMonth() + 1 + "/" + objOrderdate.getDate() + "/" + objOrderdate.getFullYear());
            //$('td:eq(5)', nRow).html(objCreateDate.getMonth() + 1 + "/" + objCreateDate.getDate() + "/" + objCreateDate.getFullYear());
        }
    });
});

function makeVendorRequestBtn(oObj) {
    var sItemID = oObj.aData.item;
    var sHref

    sHref = sVendorRequestsUrl + '?&ItemID=' + sItemID + '&RequestType=Item'; //generate the query string
    return "<a href=\"javascript:loadVendorRequestDialog('" + sHref + "')\" class='Process' title='View Vendor Requests'><img src='" + sOpenImageUrl + "' height='10' width='10'></a>";
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
            {
                "mDataProp": "QtyLoss",
                "bVisible": false
            },
            { "mDataProp": "SourceWarehouse" },
            { "mDataProp": "DestWarehouse" },
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
            },
            {
                "mDataProp": "ReleaseNo", //I don't show this row on the datatable, but I need it here so that I can populate it on my frmAddVendorRequest
                "bVisible": false
            }]
    }).makeEditable({ //I kept getting the error "Object doesn't support property or method 'live'" until I changed line 1260 of jquery.dataTables.editable.js 'live' method to 'on'
        sUpdateURL: sUpdateVRUrl,
        sAddURL: sAddVRUrl,
        sDeleteURL: sDeleteVRUrl,
        fnOnAdding: function () {//This event gets fired right before the form is submitted to the server. I can use this time to populate the variables I need to add a new record into the database such as OrderNo, OrderLine, OrderRelease, etc.
            //In order for this to work I needed to add the columns to the datatable and then hide them, put the hiddent textboxes on the frmAddVendorRequest form with the proper rel="#" attribute (where # is the column order
            //That they were added in the column instantiation. I then populate thier values here...

            //uses the GET function defined above and retrieves the values from the querystring...
            var dataValues = GET("ItemID");
            document.getElementById('Item').value = dataValues[0];
            document.getElementById('RequestCategoryID').value = 3; //Items have a request category of 3

            return true;
        },
        sAddNewRowFormId: "frmAddVendorRequest", //specifies the ID of the form that will be used to add a new row...
        "oAddNewRowFormOptions": { //These options are the same/taken from the jquery.dialog() options http://api.jqueryui.com/dialog/
            "title": "Add new Vendor Request", //This can also be done by setting the attribute 'title = "Add new Vendor Request"' in the form tag
            "height": 450,
            "width": 700
        },
        aoColumns: [
            null,
            {
                type: 'select',
                onblur: 'submit',  //values can be "ignore", "submit", or "cancel"
                //data: "{'COShipment':'COShipment', 'COLateRequest':'COLateRequest'}", 
                data: RequestCategories()
                //submit: 'Save changes'//shows a submit button
            },
            {},
            //{}, //not needed once I removed LossQty
            $.extend({}, defaultEditable, { indicator: 'Saving...' }), //create a column using defaultEditable value that is listed above, also passes an 'indicator' parameter that is specific to this instance...
            {//This is the normal way (as opposed to using above 'defaultEditable' method) of adding columns; note that all options that are available for editable() from jeditable are available here...
                tooltip: 'Click to edit',
                type: 'select',
                onblur: 'submit',
                data: AllWarehouses()
            }
        ]
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

    //This function needed to be in the same Block as the calling function in the oProcessTable.makeEditable() column call, else I would get the error 'SCRIPT5002: Function expected'. Its almost like the 
    //Script file gets detached from the DOM or something and then the column instantiation can't find the AllWarehouses() function... This took me a day to figure out!
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
        $.post(sRequestCatUrl + '?&RequestCategoryID=3', {}, function (data) {
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
        title: 'Item Requests'
    });
};