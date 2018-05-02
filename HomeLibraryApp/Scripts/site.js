$(document).ready(function () {
    $(".dropdown-trigger").dropdown();
    $('.tabs').tabs();
    $('.modal').modal({ onOpenEnd: function () { $('.tabs').tabs();}});
    $('.sidenav').sidenav();
    $('.datepicker').datepicker({
        firstDay: 1
    });
});

function onInvitationSuccess(result) {
    msg = result ? "The invitation has been sent" : "There was a problem while sending the invitation";
    console.log(msg);
    M.toast({ html: msg });
}