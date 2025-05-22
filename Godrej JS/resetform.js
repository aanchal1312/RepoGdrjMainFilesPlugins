function zx_chstatus() {
    var chStatus = Xrm.Page.getAttribute("zx_chstatus").getValue();
        if (chStatus === 128780000) { // Draft
        setAttributesdraft(128780000,0);
    } else if (chStatus === 128780001) { // Pending
        setAttributesdraft(128780000,0);
    } else if (chStatus === 128780002) { // Approved
        Xrm.Page.getAttribute("zx_chstatus").setValue(128780002);
        Xrm.Page.getAttribute("zx_mhstatus").setValue(128780001);
        setAttributesdraft(128780000,1);
    } else if (chStatus === 128780003) { // Rejected
        Xrm.Page.getAttribute("zx_chstatus").setValue(128780001);
        setAttributesdraft(128780000,0);
    }
}

function zx_mhstatus() {
    var zx_mhstatus = Xrm.Page.getAttribute("zx_mhstatus").getValue();
        if (zx_mhstatus === 128780000) { // Draft
        setAttributesdraft(128780000,2);
        Xrm.Page.getAttribute("zx_chstatus").setValue(128780001);

    } else if (zx_mhstatus === 128780001) { // Pending
        setAttributesdraft(128780000,2);
    } else if (zx_mhstatus === 128780002) { // Approved
        Xrm.Page.getAttribute("zx_mhstatus").setValue(128780002);
        Xrm.Page.getAttribute("zx_bmstatus").setValue(128780001);
        Xrm.Page.getAttribute("zx_chstatus").setValue(128780002);

        setAttributesdraft(128780000,3);
    } else if (zx_mhstatus === 128780003) { // Rejected
        Xrm.Page.getAttribute("zx_mhstatus").setValue(128780001);
        setAttributesdraft(128780000,2);
    }
}
function zx_bmstatus() {
    var zx_bmstatus = Xrm.Page.getAttribute("zx_bmstatus").getValue();
        if (zx_bmstatus === 128780000) { // Draft
        setAttributesdraft(128780000,2);
        Xrm.Page.getAttribute("zx_mhstatus").setValue(128780001);
    } else if (zx_bmstatus === 128780001) { // Pending
        setAttributesdraft(128780000,2);
        Xrm.Page.getAttribute("zx_bmstatus").setValue(128780001);
    } else if (zx_bmstatus === 128780002) { // Approved
        setAttributesdraft(128780000,3);
        Xrm.Page.getAttribute("zx_mhstatus").setValue(128780002);
        Xrm.Page.getAttribute("zx_bmstatus").setValue(128780002);
        Xrm.Page.getAttribute("zx_chstatus").setValue(128780002);
        Xrm.Page.getAttribute("zx_chstatus").setValue(128780002);
        Xrm.Page.getAttribute("zx_centralspocstatus").setValue(128780001);
    } else if (zx_bmstatus === 128780003) { // Rejected
        Xrm.Page.getAttribute("zx_bmstatus").setValue(128780001);
        setAttributesdraft(128780000,2);
    }
}


function zx_centralspocstatus() {
    var zx_centralspocstatus = Xrm.Page.getAttribute("zx_centralspocstatus").getValue();
        if (zx_centralspocstatus === 128780000) { // Draft
        setAttributesdraft(128780000,4);
        Xrm.Page.getAttribute("zx_centralspocstatus").setValue(128780001);
        Xrm.Page.getAttribute("zx_mastatus").setValue(128780000);
        
    } else if (zx_centralspocstatus === 128780001) { // Pending
        setAttributesdraft(128780000,4);
        Xrm.Page.getAttribute("zx_bmstatus").setValue(128780002);
    } else if (zx_centralspocstatus === 128780002) { // Approved
        setAttributesdraft(128780000,3);
        Xrm.Page.getAttribute("zx_mhstatus").setValue(128780002);
        Xrm.Page.getAttribute("zx_bmstatus").setValue(128780002);
        Xrm.Page.getAttribute("zx_chstatus").setValue(128780002);
        Xrm.Page.getAttribute("zx_chstatus").setValue(128780002);
        Xrm.Page.getAttribute("zx_centralspocstatus").setValue(128780002);
        Xrm.Page.getAttribute("zx_mastatus").setValue(128780001);

    } else if (zx_centralspocstatus === 128780003) { // Rejected
        Xrm.Page.getAttribute("zx_centralspocstatus").setValue(128780001);
        setAttributesdraft(128780000,4);
        Xrm.Page.getAttribute("zx_mastatus").setValue(128780000);

    }
}



function setAttributesdraft(value,no) {
    var attributes =[];

    switch(no)
    {
        case 0:
        attributes=     
        [
            "zx_mhstatus", "zx_bmstatus", "zx_mastatus", "zx_centralspocstatus",
            "zx_coestatus", "zx_documentstatus", "zx_prestatus", "zx_provplanstatus",
            "zx_estimatesstatus", "zx_invoicestatus", "zx_buyinggridstatus"
        ];
        break;
        case 1:
        attributes=
        [
         "zx_bmstatus", "zx_mastatus", "zx_centralspocstatus",
            "zx_coestatus", "zx_documentstatus", "zx_prestatus", "zx_provplanstatus",
            "zx_estimatesstatus", "zx_invoicestatus", "zx_buyinggridstatus"
        ];
        break;
        case 2:
            attributes=
            [
             "zx_bmstatus", "zx_mastatus", "zx_centralspocstatus",
                "zx_coestatus", "zx_documentstatus", "zx_prestatus", "zx_provplanstatus",
                "zx_estimatesstatus", "zx_invoicestatus", "zx_buyinggridstatus"
            ];
            break;
            case 3:
                attributes=
                [
                 "zx_mastatus", "zx_centralspocstatus",
                    "zx_coestatus", "zx_documentstatus", "zx_prestatus", "zx_provplanstatus",
                    "zx_estimatesstatus", "zx_invoicestatus", "zx_buyinggridstatus"
                ];
                break;
                case 4:
                    attributes=
                    [
                    "zx_coestatus", "zx_documentstatus", "zx_prestatus", "zx_provplanstatus",
                        "zx_estimatesstatus", "zx_invoicestatus", "zx_buyinggridstatus"
                    ];
                    break;
    



    }
        attributes.forEach(function(attr) {
            Xrm.Page.getAttribute(attr).setValue(value);
        });
        
        

    
}

function setAttributespending(value,no)
{
    attributes=
    [
     "zx_bmstatus", "zx_mastatus", "zx_centralspocstatus",
        "zx_coestatus", "zx_documentstatus", "zx_prestatus", "zx_provplanstatus",
        "zx_estimatesstatus", "zx_invoicestatus", "zx_buyinggridstatus"
    ];
   
    attributes.forEach(function(attr) {
        Xrm.Page.getAttribute(attr).setValue(value);
    });
    
}

function setAttributesApproved(value,no)
{
    attributes=
    [
     "zx_bmstatus", "zx_mastatus", "zx_centralspocstatus",
        "zx_coestatus", "zx_documentstatus", "zx_prestatus", "zx_provplanstatus",
        "zx_estimatesstatus", "zx_invoicestatus", "zx_buyinggridstatus"
    ];
   
    attributes.forEach(function(attr) {
        Xrm.Page.getAttribute(attr).setValue(value);
    });
}