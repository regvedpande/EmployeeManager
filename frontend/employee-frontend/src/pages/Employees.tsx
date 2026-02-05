import { useEffect, useState } from "react";
import EmployeeForm from "../components/EmployeeForm";
import Navbar from "../components/Navbar";
import api from "../services/api";

interface Employee {
  id: number;
  fullName: string;
  email: string;
  salary: number;
  department?: {
    name: string;
  };
}

const Employees = () => {
  const [employees, setEmployees] = useState<Employee[]>([]);
  const [showForm, setShowForm] = useState(false);
  const [editingId, setEditingId] = useState<number | null>(null);

  useEffect(() => {
    const loadEmployees = async () => {
      const res = await api.get<Employee[]>("/employees");
      setEmployees(res.data);
    };

    loadEmployees();
  }, []);

  const downloadReport = () => {
    window.open("https://localhost:7121/api/reports/employees", "_blank");
  };

  return (
    <div className="min-h-screen bg-gray-50">
      <Navbar />

      <main className="max-w-7xl mx-auto p-6">
        {/* Header */}
        <div className="flex justify-between items-center mb-6">
          <h1 className="text-2xl font-bold">Employees</h1>

          <div className="flex gap-3">
            {/* ➕ Add Employee */}
            <button
              onClick={() => {
                setEditingId(null);
                setShowForm(true);
              }}
              className="bg-indigo-600 text-white px-4 py-2 rounded-xl hover:bg-indigo-700 transition"
            >
              Add Employee
            </button>

            {/* 📊 Download Report */}
            <button
              onClick={downloadReport}
              className="bg-green-600 text-white px-4 py-2 rounded-xl hover:bg-green-700 transition"
            >
              Download Report
            </button>
          </div>
        </div>

        {/* Form */}
        {showForm && (
          <EmployeeForm
            existingId={editingId}
            onSuccess={async () => {
              const res = await api.get<Employee[]>("/employees");
              setEmployees(res.data);
              setShowForm(false);
              setEditingId(null);
            }}
            onCancel={() => {
              setShowForm(false);
              setEditingId(null);
            }}
          />
        )}

        {/* Table */}
        <div className="bg-white rounded-2xl shadow border overflow-x-auto">
          <table className="min-w-full">
            <thead className="bg-gray-100">
              <tr>
                <th className="p-4 text-left">Name</th>
                <th className="p-4 text-left">Email</th>
                <th className="p-4 text-left">Salary</th>
                <th className="p-4 text-left">Department</th>
                <th className="p-4 text-right">Actions</th>
              </tr>
            </thead>

            <tbody>
              {employees.map((emp) => (
                <tr key={emp.id} className="border-t">
                  <td className="p-4">{emp.fullName}</td>
                  <td className="p-4">{emp.email}</td>
                  <td className="p-4">
                    ₹{emp.salary.toLocaleString()}
                  </td>
                  <td className="p-4">{emp.department?.name}</td>
                  <td className="p-4 text-right">
                    <button
                      className="text-indigo-600 mr-4"
                      onClick={() => {
                        setEditingId(emp.id);
                        setShowForm(true);
                      }}
                    >
                      Edit
                    </button>
                    <button
                      className="text-red-600"
                      onClick={async () => {
                        await api.delete(`/employees/${emp.id}`);
                        setEmployees((prev) =>
                          prev.filter((e) => e.id !== emp.id)
                        );
                      }}
                    >
                      Delete
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </main>
    </div>
  );
};

export default Employees;
