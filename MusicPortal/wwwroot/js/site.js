﻿function confirmAction(url, userId, action) {
    Swal.fire({
        title: `Are you sure you want to ${action.toLowerCase()} this user?`,
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, ' + action.toLowerCase() + ' it!'
    }).then((result) => {
        if (result.isConfirmed) {
            sendPostRequest(url, userId);
        }
    });
}

function sendPostRequest(url, userId) {
    const form = document.createElement('form');
    form.method = 'post';
    form.action = url;

    const hiddenField = document.createElement('input');
    hiddenField.type = 'hidden';
    hiddenField.name = 'userId';
    hiddenField.value = userId;

    form.appendChild(hiddenField);
    document.body.appendChild(form);
    form.submit();
}
