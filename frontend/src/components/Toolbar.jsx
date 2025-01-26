import React from "react";
import { TbLockFilled } from "react-icons/tb";
import { BsUnlockFill } from "react-icons/bs";
import { MdDelete } from "react-icons/md";

const Toolbar = ({ handleDelete, handleBlock, handleUnblock }) => {
  return (
    <div className="mb-4 flex items-center gap-2 bg-white p-4 shadow-sm">
      {/* Block Button */}
      <button
        className="flex items-center gap-2 rounded bg-white border border-blue-600 px-4 py-2 text-sm font-medium text-blue-600 hover:bg-red-600 hover:text-white hover:border-red-600"
        aria-label="Block"
        onClick={handleBlock}
      >
        <TbLockFilled />
        <span>Block</span>
      </button>

      {/* Unblock Icon Button */}
      <button
        className="flex h-10 w-10 items-center justify-center rounded border border-blue-600 text-blue-600 hover:bg-green-500 hover:text-white hover:border-green-500"
        aria-label="Unblock"
        onClick={handleUnblock}
      >
        <BsUnlockFill />
      </button>

      {/* Delete Icon Button */}
      <button
        className="flex h-10 w-10 items-center justify-center rounded border border-red-500 text-red-500 hover:text-white hover:border-white hover:bg-red-700"
        aria-label="Delete"
        onClick={handleDelete}
      >
        <MdDelete />
      </button>
    </div>
  );
};

export default Toolbar;
