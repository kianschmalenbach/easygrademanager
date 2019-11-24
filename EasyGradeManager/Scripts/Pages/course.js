function initPage() {
    const studentResultTableRow = document.getElementById("StudentResultTableRow");
    if (studentResultTableRow !== null && authorizedUser.Roles.includes("Teacher"))
        studentResultTableRow.remove();
}
