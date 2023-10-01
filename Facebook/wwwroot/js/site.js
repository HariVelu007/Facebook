// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
ShowFriend = (url) => {
    $.ajax({
        type: 'GET',        
        url: url,
        success: function (res) {
            $('#FriendModal .modal-body').html(res);            
            $('#FriendModal').modal('show');   
         
        }
    })
}