﻿$(document).ready(function () {
    $(".dropdown-trigger").dropdown();
    $('.modal').modal();
});

function onInvitationSuccess(result) {
    msg = result ? "The invitation has been sent" : "There was a problem while sending the invitation";
    console.log(msg);
    M.toast({ html: msg });
}