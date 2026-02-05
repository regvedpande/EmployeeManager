import { useEffect, useState } from "react";
import api from "../services/api";

interface EmployeeFormProps {
  existingId?: number | null;
  onSuccess: () => void;
  onCancel: () => void;
}

const EmployeeForm = ({
  existingId = null,
  onSuccess,
  onCancel,
}: EmployeeFormProps) => {
  const [form, setForm] = useState({
    fullName: "",
    email: "",
    salary: "",
    departmentId: 1,
  });

  // ✅ Load employee for edit
  useEffect(() => {
    if (!existingId) return;

    const loadEmployee = async () => {
      const res = await api.get(`/employees/${existingId}`);
      setForm({
        fullName: res.data.fullName,
        email: res.data.email,
        salary: String(res.data.salary),
        departmentId: res.data.departmentId,
      });
    };

    loadEmployee();
  }, [existingId]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    const payload = {
      fullName: form.fullName,
      email: form.email,
      salary: Number(form.salary),
      departmentId: form.departmentId,
    };

    if (existingId) {
      await api.put(`/employees/${existingId}`, payload);
    } else {
      await api.post("/employees", payload);
    }

    onSuccess();
  };

  return (
    <form
      onSubmit={handleSubmit}
      className="bg-white p-6 rounded-xl shadow mb-6"
    >
      <h2 className="text-lg font-semibold mb-4">
        {existingId ? "Edit Employee" : "Add Employee"}
      </h2>

      <input
        className="w-full p-2 mb-3 border rounded"
        placeholder="Full Name"
        value={form.fullName}
        onChange={e => setForm({ ...form, fullName: e.target.value })}
        required
      />

      <input
        className="w-full p-2 mb-3 border rounded"
        placeholder="Email"
        value={form.email}
        onChange={e => setForm({ ...form, email: e.target.value })}
        required
      />

      {/* ✅ Salary input with manual typing (no arrows) */}
      <input
        type="text"
        inputMode="numeric"
        className="w-full p-2 mb-3 border rounded"
        placeholder="Salary"
        value={form.salary}
        onChange={e => {
          if (/^\d*$/.test(e.target.value)) {
            setForm({ ...form, salary: e.target.value });
          }
        }}
        required
      />

      <select
        className="w-full p-2 mb-4 border rounded"
        value={form.departmentId}
        onChange={e =>
          setForm({ ...form, departmentId: Number(e.target.value) })
        }
      >
        <option value={1}>HR</option>
        <option value={2}>Engineering</option>
        <option value={3}>Sales</option>
      </select>

      <div className="flex gap-3">
        <button
          type="submit"
          className="flex-1 bg-indigo-600 text-white py-2 rounded"
        >
          {existingId ? "Update" : "Save"}
        </button>

        <button
          type="button"
          onClick={onCancel}
          className="flex-1 bg-gray-100 py-2 rounded"
        >
          Cancel
        </button>
      </div>
    </form>
  );
};

export default EmployeeForm;
