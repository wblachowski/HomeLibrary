﻿$(document).ready(function () {
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

function scanGoodreads() {
    $("#goodreads-error").hide();
    link = $("#goodreads-input").val();
    if (link.trim() == "") {
        $('#goodreads .preloader-wrapper').hide();
        $("#goodreads-result").hide();
        return;
    }

    $('#goodreads .preloader-wrapper').show();
    $("#goodreads-result").hide();
    $.ajax({
        url: "https://cors.io/?"+link,
        success: function (result) {
            html = $.parseHTML(result);
            author = $(html).find("span[itemprop='name']").html().split(" ");
            title = $(html).find("#bookTitle").text().trim();
            publicationData = $(html).find("div:contains('Published'):not(:has(div))").text();
            publisher = publicationData.substring(publicationData.lastIndexOf("by ") + 3, publicationData.length);
            publicationDate = publicationData.substring(publicationData.lastIndexOf("Published") + 10, publicationData.lastIndexOf("by ")).trim();
            publicationDate = publicationDate.split(" ");
            if (publicationDate.length == 1) {
                month = "Jan"
                day = "01"
                year = publicationDate[0]
            } else {
                month = publicationDate[0].substring(0, 3);
                day = publicationDate[1].replace(/[^0-9]/g, '');
                if (day.length == 1) day = '0' + day;
                year = publicationDate[2];
            }


            $('#goodreads .preloader-wrapper').hide();
            $("#goodreads-result").show();

            $("#goodreadsTitle").val(title);
            $("#goodreadsFirstname").val(author[0]);
            $("#goodreadsLastname").val(author[1]);
            $("#goodreadsPublisher").val(publisher);
            $("#goodreadsDate").val(month + " " + day + ", " + year);

            M.updateTextFields();
        },
        error: function (result) {
            $('#goodreads .preloader-wrapper').hide();
            $("#goodreads-result").hide();
            $("#goodreads-error").show();
        }
    });
}