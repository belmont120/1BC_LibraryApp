import { useEffect, useState } from 'react';
import { Flex, Button, Space, Table, Input, Modal } from 'antd';
import type { TableProps } from 'antd';
import './App.css';

interface Book {
    key: string;
    bookId: string;
    title: string;
    owner: string;
    isAvailable: boolean;
}


function App() {
    const [books, setBooks] = useState<Book[]>();
    const [filter, setFilter] = useState('');
    const [filteredBooks, setFilteredBooks] = useState<Book[]>();
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [newBookTitle, setNewBookTitle] = useState('');
    const [newBookOwner, setNewBookOwner] = useState('');

    const showModal = () => {
        setIsModalOpen(true);
    };
    const handleOk = () => {
        setIsModalOpen(false);
        createBook(newBookTitle, newBookOwner);
        setNewBookTitle('');
        setNewBookOwner('');
    };
    const handleCancel = () => {
        setIsModalOpen(false);
        setNewBookTitle('');
        setNewBookOwner('');
    };
    const getBooks = async () => {
        const response = await fetch('api/books');
        if (response.ok) {
            const data = await response.json();
            return data;
        }
    };

    useEffect(() => {
        getBooks().then((books) => {
            setBooks(books);
            setFilteredBooks(books);
        });
    }, []);

    const handleFilterChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const keyword = e.target.value;

        if (keyword.length == 0) {
            setFilteredBooks(books);
            return;
        }

        setFilter(keyword);

        const lowerSearch = filter.toLowerCase();

        const results = books?.filter(book => {
            return (
                book.title.toLowerCase().includes(lowerSearch)
            );
        });

        setFilteredBooks(results);
    }

    const refresh = async () => {
        getBooks().then((books) => {
            setBooks(books);
            setFilteredBooks(books);
            setFilter('');
        });
    }

    const createBook = async (title: string, owner: string) => {
        try {
            const response = await fetch(
                `api/books`,
                {
                    method: "POST",
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify(
                        {
                            title: title,
                            owner: owner
                        })
                }
            );

            if (!response.ok) {
                throw new Error(`Failed to create book. Status: ${response.status}`);
            }

            // Update UI after successful deletion
            refresh();
        } catch (err) {
            if (err instanceof Error) {
                alert(`Error: ${err.message}`);
            } else {
                console.log("Unexpected error type:", err);
            }
        }
    };

    const borrowBook = async (bookId: string) => {
        try {
            const response = await fetch(
                `api/books/${bookId}/borrow`,
                {
                    method: "POST",
                }
            );

            if (!response.ok) {
                throw new Error(`Failed to borrow book. Status: ${response.status}`);
            }

            // Update UI after successful deletion
            refresh();
        } catch (err) {
            if (err instanceof Error) {
                alert(`Error: ${err.message}`);
            } else {
                console.log("Unexpected error type:", err);
            }
        }
    };

    const returnBook = async (bookId: string) => {
        try {
            const response = await fetch(
                `api/books/${bookId}/return`,
                {
                    method: "POST",
                }
            );

            if (!response.ok) {
                throw new Error(`Failed to return book. Status: ${response.status}`);
            }

            // Update UI after successful deletion
            refresh();
        } catch (err) {
            if (err instanceof Error) {
                alert(`Error: ${err.message}`);
            } else {
                console.log("Unexpected error type:", err);
            }
        }
    };

    const deleteBook = async (bookId: string) => {
        if (!window.confirm("Are you sure you want to delete this user?")) return;

        try {
            const response = await fetch(
                `api/books/${bookId}`,
                {
                    method: "DELETE",
                }
            );

            if (!response.ok) {
                throw new Error(`Failed to delete book. Status: ${response.status}`);
            }

            // Update UI after successful deletion
            refresh();
        } catch (err) {
            if (err instanceof Error) {
                alert(`Error: ${err.message}`);
            } else {
                console.log("Unexpected error type:", err);
            }
        }
    };

    const columns: TableProps<Book>['columns'] = [
        {
            title: 'Book',
            dataIndex: 'title',
            key: 'title'
        },
        {
            title: 'Owner',
            dataIndex: 'owner',
            key: 'owner',
        },
        {
            title: 'Availability',
            dataIndex: 'isAvailable',
            key: 'isAvailable',
            render: (isAvailable) => <p>{isAvailable ? 'Available' : 'Unavailable'}</p>,
        },
        {
            title: 'Action',
            key: 'action',
            render: (_, record) => (
                <Space size="small">
                    <Button disabled={!record.isAvailable} onClick={() => borrowBook(record.bookId)}>Borrow</Button>
                    <Button disabled={record.isAvailable} onClick={() => returnBook(record.bookId)}>Return</Button>
                    <Button danger onClick={() => deleteBook(record.bookId)}>Delete</Button>
                </Space>
            ),
        },
    ];

    const bookTable = books === undefined
        ? <p><em>Loading... Please refresh once the ASP.NET backend has started.</em></p>
        : <Table<Book> columns={columns} dataSource={filteredBooks} />;;

    return (
        <Flex gap="medium" vertical>
            <h1 id="tableLabel">Library</h1>
            <Input placeholder="Search..." onChange={handleFilterChange} />
            {bookTable}

            <Button
                size="large"
                style={{
                    position: "fixed",
                    bottom: 20, // distance from bottom
                    right: 20,  // distance from right
                    zIndex: 1000 // ensure it stays above other elements
                }}
                onClick={() => showModal()}>Add Book</Button>
            <Modal
                title="Create new book"
                closable={{ 'aria-label': 'Custom Close Button' }}
                open={isModalOpen}
                onOk={handleOk}
                onCancel={handleCancel}
            >
                <h5>Title</h5>
                <Input placeholder="new book title"
                    value={newBookTitle}
                    onChange={e => setNewBookTitle(e.target.value)} />
                <h5>Owner</h5>
                <Input placeholder="new book owner"
                    value={newBookOwner}
                    onChange={e => setNewBookOwner(e.target.value)} />
            </Modal>
        </Flex>
    );
}

export default App;