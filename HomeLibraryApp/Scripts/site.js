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

function searchForBooks(page) {
    searchType = 'All'
    searchQuery = '';
    if (page == null) {
        page = 1;
    }
    $("[name='searchGroup'").each(function (index, element) {
        if ($(this).is(":checked")) {
            searchType = $(this).next().html();
        }
    });
    searchQuery = $('#search-book-input').val();

    $.ajax({
        url: "GetSearchedBooks",
        data: {
            searchType: searchType,
            query: searchQuery,
            page: page
        },
        success: function (result) {
            $('#search-book-results').html(result);
        }
    });
}