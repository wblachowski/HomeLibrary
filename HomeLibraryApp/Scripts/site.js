$(document).ready(function () {
    $(".dropdown-trigger").dropdown();
    $('.tabs').tabs();
    $('.modal').modal({ onOpenEnd: function () { $('.tabs').tabs(); } });
    $('.sidenav').sidenav();
    $('.datepicker').datepicker({
        firstDay: 1
    });
    $('select').formSelect();

    $(".clickable-row").click(function () {
        window.location = $(this).data("href");
    });

    if ($("#goodreads-input") !== null && $("#goodreads-input").val() !== "") {
        $("#goodreads-result").show();
    }
    if ($("#search-book-input").length > 0 && $("#search-book-input").val() !== "") {
        searchForBooks(1, 'search');
    }
    $('#borrowedCheckbox').change(function () {
        if ($(this).is(":checked")) {
            $("#borrowedData").slideDown("fast");
        } else {
            $("#borrowedData").slideUp("fast");
        }
    });
    if (window.innerWidth >= 600) {
        setCardsHeight();
    }
    window.addEventListener('resize', function (event) {
        if (window.innerWidth >= 600) {
            setCardsHeight();
        } else {
            $("#last-book-card").css('min-height', '');
            $("#stats-card").css('min-height', '');
        }
    });
});

function setCardsHeight() {
    var cardsHeight = Math.max($("#last-book-card").height(), $("#stats-card").height())
    $("#last-book-card").css('min-height', cardsHeight);
    $("#stats-card").css('min-height', cardsHeight);
}

function addFormSubmit(source) {
    if ($('#borrowedCheckbox').is(":checked")) {
        $(source.form).find("[name='LenderFirstname']").val($("#LenderFirstname").val());
        $(source.form).find("[name='LenderLastname']").val($("#LenderLastname").val());
    }
    source.form.submit();
}

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



function searchBookAddClick(id, sourceView) {
    if (sourceView == 'search') {
        $('#search-book-select-library').modal();
        $('#search-book-select-library').modal('open');
        $('select').formSelect();
        $('#modal-details-id').val(id);

    } else {
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
}

function scanGoodreads() {
    $("#goodreads-error").hide();
    link = $("#goodreads-input").val();
    if (link.trim() === "") {
        $('#goodreads .preloader-wrapper').hide();
        $("#goodreads-result").hide();
        return;
    }

    $('#goodreads .preloader-wrapper').show();
    $("#goodreads-result").hide();
    $.ajax({
        url: "https://cors.io/?" + link,
        success: function (result) {
            html = $.parseHTML(result);
            author = $(html).find("span[itemprop='name']").html().split(" ");
            title = $(html).find("#bookTitle").clone().children().remove().end().text().trim();
            publicationData = $(html).find("div:contains('Published'):not(:has(div))").clone().children().remove().end().text();
            publisher = publicationData.substring(publicationData.lastIndexOf("by ") + 3, publicationData.length);
            publicationDate = publicationData.substring(publicationData.lastIndexOf("Published") + 10, publicationData.lastIndexOf("by ")).trim();
            publicationDate = publicationDate.split(" ");
            if (publicationDate.length === 1) {
                month = "Jan"
                day = "01"
                year = publicationDate[0]
            } else {
                month = publicationDate[0].substring(0, 3);
                day = publicationDate[1].replace(/[^0-9]/g, '');
                if (day.length === 1) day = '0' + day;
                year = publicationDate[2];
            }


            $('#goodreads .preloader-wrapper').hide();
            $("#goodreads-result").show();

            $("#GoodreadsBookModel_Title").attr('value', title);
            $("#GoodreadsBookModel_AuthorFirstname").attr('value', author[0]);
            $("#GoodreadsBookModel_AuthorLastname").attr('value', author[1]);
            $("#GoodreadsBookModel_Publisher").attr('value', publisher);
            $("#GoodreadsBookModel_PublicationDate").attr('value', month + " " + day + ", " + year);

            $("#GoodreadsBookModel_Title").change();
            $("#GoodreadsBookModel_AuthorFirstname").change();
            $("#GoodreadsBookModel_AuthorLastname").change();
            $("#GoodreadsBookModel_Publisher").change();
            $("#GoodreadsBookModel_PublicationDate").change();


            M.updateTextFields();
        },
        error: function (result) {
            $('#goodreads .preloader-wrapper').hide();
            $("#goodreads-result").hide();
            $("#goodreads-error").show();
        }
    });
}

function onClickSearchTopbar() {
    var query = $("#topbarsearch-input").val();
    var url = '/Library/Search';
    if (query !== "") {
        window.location = url + '?q=' + query;
    } else {
        window.location = url;
    }
}

function searchAddBookNext() {
    var lib = $("#library-select").children().find("option:selected").attr("value");
    $("#confirm-form").attr("action", "/Library/Add?type=existing&lib=" + lib + "&source=search")
    if ($('#borrowedCheckbox').is(":checked")) {
        $("[name='LenderFirstname']").val($("#LenderFirstname").val());
        $("[name='LenderLastname']").val($("#LenderLastname").val());
    }
    id = $('#modal-details-id').val();
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
    $('#search-book-select-library').modal('destroy');
    $('#search-book-confirm-modal').modal('open');

}