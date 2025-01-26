import React, { useState, useEffect } from "react";
import TableRow from "./TableRow";
import Toolbar from "./Toolbar";
import toast from "react-hot-toast";
import { useNavigate } from "react-router-dom";

const Home = () => {
  const [user, setUser] = useState({});
  const [users, setUsers] = useState([]);
  const [loading, setLoading] = useState(false);
  const [token, settoken] = useState(localStorage.getItem("token") || "");
  const navigate = useNavigate();

  const fetchUser = async () => {
    const email = localStorage.getItem("email");
    try {
      const response = await fetch(
        `${import.meta.env.VITE_BACKEND_URL}/users/email/${email}`,
        {
          method: "GET",
          headers: {
            Authorization: `Bearer ${token}`,
            "Content-Type": "application/json",
          },
        }
      );
      const data = await response.json();
      if (response.ok) {
        if (data.status === "blocked") {
          navigate("/login");
        }
        setUser(data);
      } else {
        navigate("/login");
      }
    } catch (error) {
      console.error(error.error);
      toast.error(error.error);
      navigate("/login");
    }
  };

  useEffect(() => {
    fetchUser();
  }, []);

  useEffect(() => {
    const fetchUsers = async () => {
      try {
        const response = await fetch(
          `${import.meta.env.VITE_BACKEND_URL}/users`,
          {
            method: "GET",
            headers: {
              Authorization: `Bearer ${token}`,
              "Content-Type": "application/json",
            },
          }
        );
        const data = await response.json();
        if (response.ok) {
          const updatedUsers = data
            .map((user) => ({
              ...user,
              isSelected: false,
            }))
            .sort((a, b) => new Date(b.lastSeen) - new Date(a.lastSeen));
          setUsers(updatedUsers);
        } else {
          navigate("/login");
        }
      } catch (error) {
        console.error(error.error);
        toast.error(error.error);
      }
    };

    fetchUsers();
  }, []);

  const handleDelete = async (e) => {
    e.preventDefault();
    if (loading) return;
    const userIds = users
      .filter((user) => user.isSelected)
      .map((user) => user.id);
    if (userIds.length === 0) {
      return toast.error("No user is selected.");
    }
    setLoading(true);
    try {
      const response = await fetch(
        `${import.meta.env.VITE_BACKEND_URL}/users/${user.id}`,
        {
          method: "DELETE",
          headers: {
            Authorization: `Bearer ${token}`,
            "Content-Type": "application/json",
          },
          body: JSON.stringify({ userIds: userIds }),
        }
      );
      const data = await response.text();
      if (response.ok) {
        toast.success("The selected users have been deleted.");
        setUsers((prev) => prev.filter((user) => !user.isSelected));
        fetchUser();
      } else {
        toast.error(data.error);
        navigate("/login");
      }
    } catch (error) {
      console.error(error.error);
      toast.error(error.error);
    } finally {
      setLoading(false);
    }
  };

  const handleBlock = async (e) => {
    e.preventDefault();
    if (loading) return;
    const userIds = users
      .filter((user) => user.isSelected)
      .map((user) => user.id);
    if (userIds.length === 0) {
      return toast.error("No user is selected.");
    }
    setLoading(true);
    try {
      const response = await fetch(
        `${import.meta.env.VITE_BACKEND_URL}/users/block/${user.id}`,
        {
          method: "PATCH",
          headers: {
            Authorization: `Bearer ${token}`,
            "Content-Type": "application/json",
          },
          body: JSON.stringify({ userIds: userIds }),
        }
      );
      const data = await response.text();
      if (response.ok) {
        toast.success("The selected users have been blocked.");
        setUsers((prev) =>
          prev.map((user) =>
            user.isSelected === true ? { ...user, status: "blocked" } : user
          )
        );
        fetchUser();
      } else {
        toast.error(data.error);
        navigate("/login");
      }
    } catch (error) {
      console.error(error.error);
      toast.error(error.error);
    } finally {
      setLoading(false);
    }
  };

  const handleUnblock = async (e) => {
    if (loading) return;
    e.preventDefault();
    const userIds = users
      .filter((user) => user.isSelected)
      .map((user) => user.id);
    if (userIds.length === 0) {
      return toast.error("No user is selected.");
    }
    setLoading(true);
    try {
      const response = await fetch(
        `${import.meta.env.VITE_BACKEND_URL}/users/activate/${user.id}`,
        {
          method: "PATCH",
          headers: {
            Authorization: `Bearer ${token}`,
            "Content-Type": "application/json",
          },
          body: JSON.stringify({ userIds: userIds }),
        }
      );
      const data = await response.text();
      if (response.ok) {
        toast.success("The selected users have been unblocked.");
        setUsers((prev) =>
          prev.map((user) =>
            user.isSelected === true ? { ...user, status: "active" } : user
          )
        );
        fetchUser();
      } else {
        toast.error(data.error);
        navigate("/login");
      }
    } catch (error) {
      console.error(error.error);
      toast.error(error.error);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="max-h-screen overflow-y-auto pt-10">
      <Toolbar
        handleDelete={handleDelete}
        handleBlock={handleBlock}
        handleUnblock={handleUnblock}
      />
      <TableRow individuals={users} setIndividuals={setUsers} />
    </div>
  );
};

export default Home;
