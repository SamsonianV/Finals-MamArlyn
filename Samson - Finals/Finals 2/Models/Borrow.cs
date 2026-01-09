namespace Finals_2.Models
{
    public class Borrow
    {
        public int BorrowID { get; set; }
        public int EmployeeID { get; set; }
        public int EquipmentID { get; set; }
        public string? EmployeeName { get; set; }
        public string? EquipmentName { get; set; }
        public DateTime BorrowDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public string? Status { get; set; }
    }

}
