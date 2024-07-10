function saveTabState() {
    const activeTab = document.querySelector('.nav-tabs .nav-link.active').getAttribute('id');
    localStorage.setItem('activeTab', activeTab);
}


function restoreTabState() {
    const activeTab = localStorage.getItem('activeTab');
    if (activeTab) {
        const tab = document.getElementById(activeTab);
        if (tab) {
            tab.click();
        }
    }
}


function confirmAction(url, userId, action) {
    saveTabState();

    Swal.fire({
        title: `Are you sure you want to ${action.toLowerCase()} this user?`,
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#48e945',
        cancelButtonColor: '#e91212',
        customClass: {
            container: 'swal-container',
            popup: 'swal-popup',
            title: 'swal-title',
            confirmButton: 'confirm-btn',
            cancelButton: 'cancel-btn'
        },
        confirmButtonText: 'Yes, ' + action.toLowerCase() + ' it!'
    }).then((result) => {
        if (result.isConfirmed) {
            sendPostRequest(url, userId);
        }
    });
}

async function sendPostRequest(url, userId) {
    try {
        const response = await fetch(url, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded'
            },
            body: new URLSearchParams({ 'userId': userId })
        });

        if (response.ok) {
            await updateTabContent();
        } else {
            Swal.fire('Error', 'There was an error processing your request.', 'error');
        }
    } catch (error) {
        console.error('Error:', error);
        Swal.fire('Error', 'There was an error processing your request.', 'error');
    }
}

async function updateTabContent() {
    try {
        const confirmationTab = document.querySelector('#confirmation');
        const managementTab = document.querySelector('#management');

        const confirmationResponse = await fetch('/Admin/UserConfirmationPartial');
        const managementResponse = await fetch('/Admin/UserManagementPartial');

        if (confirmationResponse.ok) {
            const confirmationHtml = await confirmationResponse.text();
            confirmationTab.innerHTML = confirmationHtml;
        }

        if (managementResponse.ok) {
            const managementHtml = await managementResponse.text();
            managementTab.innerHTML = managementHtml;
        }

        restoreTabState();
    } catch (error) {
        console.error('Error:', error);
    }
}
