const borrowedList = document.getElementById("borrowedList");
const equipmentSelect = document.getElementById("equipmentSelect");
const employeeSelect = document.getElementById("employeeSelect");
const borrowBtn = document.getElementById("borrowBtn");
const returnBtn = document.getElementById("returnBtn");
const returnIdInput = document.getElementById("returnId");
const expectedReturnDateInput = document.getElementById("expectedReturnDate");

async function fetchBorrowed() {
    const res = await fetch("/api/EquipmentBorrowing/borrowed");
    const data = await res.json();
    borrowedList.innerHTML = "";
    data.forEach(b => {
        const row = document.createElement("div");
        row.className = "listview-row";
        row.innerHTML = `
        <div>${b.borrowID}</div>
        <div>${b.employeeName}</div>
        <div>${b.equipmentName}</div>
        <div>${new Date(b.borrowDate).toLocaleDateString()}</div>
        <div>${b.returnDate ? new Date(b.returnDate).toLocaleDateString() : ""}</div>
    `;
        borrowedList.appendChild(row);
    });
}

async function fetchEmployees() {
    const res = await fetch("/api/EquipmentBorrowing/employees");
    const data = await res.json();
    employeeSelect.innerHTML = `<option value="">Select Employee</option>`;
    data.forEach(e => {
        const option = document.createElement("option");
        option.value = e.employeeID;
        option.textContent = e.employeeName;
        employeeSelect.appendChild(option);
    });
}

async function fetchEquipment() {
    const res = await fetch("/api/EquipmentBorrowing/equipment");
    const data = await res.json();
    equipmentSelect.innerHTML = `<option value="">Select Equipment</option>`;
    data.forEach(eq => {
        const option = document.createElement("option");
        option.value = eq.equipmentID;
        option.textContent = `${eq.equipmentName} (${eq.quantity})`;
        equipmentSelect.appendChild(option);
    });
}

borrowBtn.addEventListener("click", async () => {
    const employeeId = parseInt(employeeSelect.value);
    const equipmentId = parseInt(equipmentSelect.value);
    const returnDate = expectedReturnDateInput.value;
    if (!employeeId || !equipmentId || !returnDate) return alert("Select employee, equipment and return date");

    const res = await fetch("/api/EquipmentBorrowing/borrow", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
            EmployeeID: employeeId,
            EquipmentID: equipmentId,
            ReturnDate: returnDate
        })
    });
    employeeSelect.value = '';
    expectedReturnDateInput.value = '';
    const text = await res.text();
    alert(text);
    fetchBorrowed();
    fetchEquipment();
});

returnBtn.addEventListener("click", async () => {
    const borrowId = parseInt(returnIdInput.value);
    if (!borrowId) return alert("Enter Borrow ID");

    const res = await fetch(`/api/EquipmentBorrowing/return/${borrowId}`, { method: "PUT" });
    const text = await res.text();
    alert(text);
    returnIdInput.value = "";
    fetchBorrowed();
    fetchEquipment();
});

window.addEventListener("load", () => {
    fetchBorrowed();
    fetchEmployees();
    fetchEquipment();
});
