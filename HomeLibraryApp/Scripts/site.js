$(document).ready(function () {
    $(".dropdown-trigger").dropdown();
    $('.tabs').tabs();
    $('.modal').modal({ onOpenEnd: function () { $('.tabs').tabs();}});
    $('.sidenav').sidenav();
    $('.datepicker').datepicker({
        firstDay: 1
    });
});

function onInvitationComplete(result) {
    msg = result ? "The invitation has been sent" : "There was a problem while sending the invitation";
    console.log(msg);
    M.toast({ html: msg });
}

function onNewBookAddedComplete(result) {
    msg = result ? "New book has been added" : "There was a problem while adding the book";
    console.log(msg);
    M.toast({ html: msg });
}



function searchBookAddClick(id) {
    rowId = "bookRow" + id;
    console.log(rowId);
    rowData = $("#" + rowId + " td");
    $("#modalId").val(id);
    $("#modalTitle").val(rowData.eq(0).html());
    $("#modalFirstname").val(rowData.eq(1).html());
    $("#modalLastname").val(rowData.eq(2).html());
    $("#modalDate").val($("#" + rowId).attr("data-date"));
    $("#modalPublisher").val(rowData.eq(3).html());

    M.updateTextFields();
    $('#search-book-confirm-modal').modal();
    $('#search-book-confirm-modal').modal('open');
}