using Finals_2.Data;
using Finals_2.Models;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace Finals_2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EquipmentBorrowingController : ControllerBase
    {
        private readonly Db _db = new Db();

        [HttpGet("borrowed")]
        public IActionResult GetBorrowed()
        {
            var list = new List<Borrow>();
            using var conn = _db.GetConnection();
            conn.Open();
            string sql = @"
                SELECT 
                    b.BorrowID,
                    e.Name AS EmployeeName,
                    eq.EquipmentName,
                    b.BorrowDate,
                    b.ReturnDate,
                    b.Status
                FROM BorrowLogs b
                JOIN Employees e ON b.EmployeeID = e.EmployeeID
                JOIN Equipment eq ON b.EquipmentID = eq.EquipmentID
                WHERE b.Status='Borrowed'
                ORDER BY b.BorrowDate DESC";
            using var cmd = new MySqlCommand(sql, conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new Borrow
                {
                    BorrowID = reader.GetInt32("BorrowID"),
                    EmployeeName = reader.GetString("EmployeeName"),
                    EquipmentName = reader.GetString("EquipmentName"),
                    BorrowDate = reader.GetDateTime("BorrowDate"),
                    ReturnDate = reader.GetDateTime("ReturnDate"),
                    Status = reader.GetString("Status")
                });
            }
            return Ok(list);
        }

        [HttpGet("employees")]
        public IActionResult GetEmployees()
        {
            var list = new List<object>();
            using var conn = _db.GetConnection();
            conn.Open();
            string sql = "SELECT EmployeeID, Name AS EmployeeName FROM Employees";
            using var cmd = new MySqlCommand(sql, conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new
                {
                    EmployeeID = reader.GetInt32("EmployeeID"),
                    EmployeeName = reader.GetString("EmployeeName")
                });
            }
            return Ok(list);
        }

        [HttpGet("equipment")]
        public IActionResult GetEquipment()
        {
            var list = new List<object>();
            using var conn = _db.GetConnection();
            conn.Open();
            string sql = "SELECT EquipmentID, EquipmentName, Quantity FROM Equipment";
            using var cmd = new MySqlCommand(sql, conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new
                {
                    EquipmentID = reader.GetInt32("EquipmentID"),
                    EquipmentName = reader.GetString("EquipmentName"),
                    Quantity = reader.GetInt32("Quantity")
                });
            }
            return Ok(list);
        }


        [HttpPost("borrow")]
        public IActionResult BorrowEquipment([FromBody] Borrow borrow)
        {
            if (borrow.EmployeeID <= 0 || borrow.EquipmentID <= 0 || borrow.ReturnDate == default)
                return BadRequest("Invalid input");

            using var conn = _db.GetConnection();
            conn.Open();
            string qtySql = "SELECT Quantity FROM Equipment WHERE EquipmentID=@id";
            using var qtyCmd = new MySqlCommand(qtySql, conn);
            qtyCmd.Parameters.AddWithValue("@id", borrow.EquipmentID);
            var quantity = Convert.ToInt32(qtyCmd.ExecuteScalar());
            if (quantity <= 0) return BadRequest("Equipment not available");

            string insertSql = @"
                INSERT INTO BorrowLogs (EmployeeID, EquipmentID, BorrowDate, ReturnDate, Status)
                VALUES (@emp, @eq, @borrowDate, @returnDate, 'Borrowed')";
            using var insertCmd = new MySqlCommand(insertSql, conn);
            insertCmd.Parameters.AddWithValue("@emp", borrow.EmployeeID);
            insertCmd.Parameters.AddWithValue("@eq", borrow.EquipmentID);
            insertCmd.Parameters.AddWithValue("@borrowDate", DateTime.Now.Date);
            insertCmd.Parameters.AddWithValue("@returnDate", borrow.ReturnDate);
            insertCmd.ExecuteNonQuery();

            string updateQtySql = "UPDATE Equipment SET Quantity = Quantity - 1 WHERE EquipmentID=@id";
            using var updateCmd = new MySqlCommand(updateQtySql, conn);
            updateCmd.Parameters.AddWithValue("@id", borrow.EquipmentID);
            updateCmd.ExecuteNonQuery();

            return Ok("Equipment borrowed successfully");
        }

        [HttpPut("return/{borrowId}")]
        public IActionResult ReturnEquipment(int borrowId)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            string checkSql = "SELECT EquipmentID FROM BorrowLogs WHERE BorrowID=@id AND Status='Borrowed'";
            int equipmentId;
            using (var checkCmd = new MySqlCommand(checkSql, conn))
            {
                checkCmd.Parameters.AddWithValue("@id", borrowId);
                using var reader = checkCmd.ExecuteReader();
                if (!reader.Read()) return NotFound("Borrow record not found");
                equipmentId = reader.GetInt32("EquipmentID");
            }

            string updateSql = "UPDATE BorrowLogs SET Status='Returned' WHERE BorrowID=@id";
            using (var updateCmd = new MySqlCommand(updateSql, conn))
            {
                updateCmd.Parameters.AddWithValue("@id", borrowId);
                updateCmd.ExecuteNonQuery();
            }

            string qtySql = "UPDATE Equipment SET Quantity = Quantity + 1 WHERE EquipmentID=@id";
            using (var qtyCmd = new MySqlCommand(qtySql, conn))
            {
                qtyCmd.Parameters.AddWithValue("@id", equipmentId);
                qtyCmd.ExecuteNonQuery();
            }

            return Ok("Equipment returned successfully");
        }
    }
}
