// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$('#btn-publish-project').on('click', function (event) {
    let selButton = $('#btn-publish-project');
    let formData = $('#form-details').serialize();
    let actionUrl = "https://localhost:44373/Projects/Edit/";

    console.log(formData);
        
    selButton.attr('disabled', 'disabled');
    selButton.val('Please wait...');
    $.ajax({
        url: actionUrl,
        type: 'post',
        data: formData
    }).done(function () {
            $('#input-publish-project').remove();
        selButton.remove();
        $('#span-project-status').text('Active');

        alert("Successfull publishing of the project.");
    }).fail(function () {
        selButton.removeAttr('disabled');
        selButton.val('Publish');
        alert("Failed to publis user's project");
    });
});