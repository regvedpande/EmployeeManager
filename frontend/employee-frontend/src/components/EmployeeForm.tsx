import { useEffect, useState } from "react";
import api from "../services/api";

interface Props {
  existingId?: number | null;
  onSuccess: () => void;
  onCancel: () => void;
}

const EmployeeForm = ({ existingId, onSuccess, onCancel }: Props) => {
  const [form, setForm] = useState({
    id: 0,
    fullName: "",
    email: "",
    salary: 0,
    departmentId: 1,
  });

  useEffect(() => {
    if (existingId) {
      api.get(`/employees/${existingId}`).then(res => {
        setForm(res.data);
      });
    }
  }, [existingId]);

  const submit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (existingId) {
      await api.put(`/employees/${existingId}`, form);
    } else {
      await api.post("/employees", form);
    }

    onSuccess();
  };

  return (
    <div className="bg-white p-8 rounded-2xl shadow border">
      <h2 className="text-xl font-bold mb-6">
        {existingId ? "Edit Employee" : "Add New Employee"}
      </h2>

      <form onSubmit={submit} className="space-y-5">
        <input
          className="w-full border p-3 rounded-xl"
          placeholder="Full Name"
          value={form.fullName}
          onChange={e => setForm({ ...form, fullName: e.target.value })}
          required
        />

        <input
          type="email"
          className="w-full border p-3 rounded-xl"
          placeholder="Email"
          value={form.email}
          onChange={e => setForm({ ...form, email: e.target.value })}
          required
        />

        <div className="grid grid-cols-2 gap-4">
          <input
            type="number"
            className="border p-3 rounded-xl"
            placeholder="Salary"
            value={form.salary}
            onChange={e => setForm({ ...form, salary: +e.target.value })}
          />

          <select
            className="border p-3 rounded-xl"
            value={form.departmentId}
            onChange={e => setForm({ ...form, departmentId: +e.target.value })}
          >
            <option value={1}>HR</option>
            <option value={2}>Engineering</option>
            <option value={3}>Sales</option>
          </select>
        </div>

        <div className="flex gap-4">
          <button className="flex-1 bg-indigo-600 text-white py-3 rounded-xl">
            Save
          </button>
          <button
            type="button"
            onClick={onCancel}
            className="flex-1 bg-gray-100 py-3 rounded-xl"
          >
            Cancel
          </button>
        </div>
      </form>
    </div>
  );
};

export default EmployeeForm;
