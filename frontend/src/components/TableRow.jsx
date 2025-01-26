import React, { useState } from "react";

const TableRow = ({ individuals, setIndividuals }) => {
  function formatDate(timestamp) {
    const date = new Date(timestamp);

    if (isNaN(date)) {
      throw new Error("Invalid timestamp");
    }

    const options = {
      year: "numeric",
      month: "long",
      day: "numeric",
      hour: "2-digit",
      minute: "2-digit",
      hour12: true,
    };

    return date.toLocaleString("en-US", options);
  }

  const handleSelectAll = (e) => {
    const isChecked = e.target.checked;
    setIndividuals((prev) =>
      prev.map((individual) => ({ ...individual, isSelected: isChecked }))
    );
  };

  const handleIndividualCheck = (index, isChecked) => {
    setIndividuals((prev) =>
      prev.map((individual, i) =>
        i === index ? { ...individual, isSelected: isChecked } : individual
      )
    );
  };

  return (
    <div className="overflow-x-auto">
      <table className="min-w-full divide-y-2 divide-gray-200 bg-white text-sm">
        <thead>
          <tr>
            <th className="sticky inset-y-0 start-0 bg-white px-4 py-2">
              <label htmlFor="SelectAll" className="sr-only">
                Select All
              </label>

              <input
                type="checkbox"
                id="SelectAll"
                onChange={handleSelectAll}
                className="size-5 rounded border-gray-300"
              />
            </th>
            <th className="whitespace-nowrap px-4 py-2 font-medium text-gray-900">
              Name
            </th>
            <th className="whitespace-nowrap px-4 py-2 font-medium text-gray-900">
              Email
            </th>
            <th className="whitespace-nowrap px-4 py-2 font-medium text-gray-900">
              Last Login
            </th>
            <th className="whitespace-nowrap px-4 py-2 font-medium text-gray-900">
              Status
            </th>
          </tr>
        </thead>

        <tbody className="divide-y divide-gray-200">
          {individuals.map((individual, index) => (
            <tr key={index}>
              <td className="sticky inset-y-0 start-0 bg-white px-4 py-2">
                <label className="sr-only" htmlFor="Row1">
                  Row {index}
                </label>

                <input
                  className="size-5 rounded border-gray-300"
                  type="checkbox"
                  id={`input-${index}`}
                  checked={individual.isSelected}
                  onChange={(e) =>
                    handleIndividualCheck(index, e.target.checked)
                  }
                />
              </td>
              <td className="whitespace-nowrap px-4 py-2 font-medium text-gray-900">
                {individual.name}
              </td>
              <td className="whitespace-nowrap px-4 py-2 text-gray-700">
                {individual.email}
              </td>
              <td className="whitespace-nowrap px-4 py-2 text-gray-700">
                {formatDate(individual.lastSeen)}
              </td>
              <td className="whitespace-nowrap px-4 py-2 text-gray-700">
                {individual.status}
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
};

export default TableRow;
