import { useEffect, useState } from "react";
import EmployeeForm from "../components/EmployeeForm";
import Navbar from "../components/Navbar";
import api from "../services/api";

interface Employee {
  id: number;
  fullName: string;
  email: string;
  department?: { name: string };
}

const Employees = () => {
  const [employees, setEmployees] = useState<Employee[]>([]);
  const [showForm, setShowForm] = useState(false);
  const [editingId, setEditingId] = useState<number | null>(null);

  const load = async () => {
    const res = await api.get("/employees");
    setEmployees(res.data);
  };

  useEffect(() => {
    load();
  }, []);

  return (
    <div className="min-h-screen bg-gray-50">
      <Navbar />

      <main className="max-w-7xl mx-auto p-6">
        <div className="flex justify-between mb-6">
          <h1 className="text-2xl font-bold">Employees</h1>

          <div className="flex gap-3">
            <button
              onClick={() => {
                setEditingId(null);
                setShowForm(true);
              }}
              className="bg-indigo-600 text-white px-4 py-2 rounded-xl"
            >
              Add Employee
            </button>

            <button
              onClick={() =>
                window.open("https://localhost:7121/api/reports/employees")
              }
              className="bg-green-600 text-white px-4 py-2 rounded-xl"
            >
              Download Report
            </button>
          </div>
        </div>

        {showForm && (
          <EmployeeForm
            existingId={editingId}
            onSuccess={() => {
              load();
              setShowForm(false);
              setEditingId(null);
            }}
            onCancel={() => {
              setShowForm(false);
              setEditingId(null);
            }}
          />
        )}

        <table className="w-full bg-white rounded-xl shadow mt-6">
          <thead className="bg-gray-100">
            <tr>
              <th className="p-4 text-left">Name</th>
              <th className="p-4 text-left">Email</th>
              <th className="p-4 text-left">Department</th>
              <th className="p-4 text-right">Actions</th>
            </tr>
          </thead>

          <tbody>
            {employees.map(e => (
              <tr key={e.id} className="border-t">
                <td className="p-4">{e.fullName}</td>
                <td className="p-4">{e.email}</td>
                <td className="p-4">{e.department?.name}</td>
                <td className="p-4 text-right">
                  <button
                    onClick={() => {
                      setEditingId(e.id);
                      setShowForm(true);
                    }}
                    className="text-indigo-600 mr-4"
                  >
                    Edit
                  </button>

                  <button
                    onClick={async () => {
                      await api.delete(`/employees/${e.id}`);
                      load();
                    }}
                    className="text-red-600"
                  >
                    Delete
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </main>
    </div>
  );
};

export default Employees;
