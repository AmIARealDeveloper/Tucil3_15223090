# Tugas Kecil 3 IF2211 Strategi Algoritma 2024/2025

### *Penyelesaian Puzzle Rush Hour Menggunakan Algoritma Pathfinding*

### Penjelasan singkat algoritma yang diimplementasikan
#### Greedy Best First Search
Algoritma ini melakukan pencarian dengan memilih langkah "terbaik" saat itu berdasarkan heuristik, seperti memilih jalur yang paling dekat dengan jalan keluar. Algoritma ini cepat menemukan solusi tetapi belum tentu optimal untuk menemukan langkah terbaik.

#### Uniform Cost Search (UCS)
Algoritma ini melakukan pencarian dengan memilih langkah yang memiliki total cost atau biaya terkecil dari awal permainan. Algortima ini menggunakan prinsip Breadth-First-Search (BFS) dengan memprioristaskan gerakan terefisien. Akan tetapi, algoritma ini juag perlu waktu lebih lama untuk memeriksa semua kemungkinan dengan biaya rendah sebelum lanjut ke langkah berikutnya.

#### A*
Algoritma ini melakukan pencarian dengan mempertimbangkan biaya yang sudah dikeluarkan (g(n)) dan prediksi heuristik menuju tujuan exit (h(n)). A* bisa dibilang merupakan kombinasi dari algoritma Greedy Best First Search dan Uniform Cost Search, dengan rumus f(n) = g(n) + h(n) sehingga A* dapat menemukan solusi optimal yang lebih efisien. 

### Requirement
1. OS Windows atau Linux
2. Install .NET SDK

### Cara menjalankan program
1. Clone repository
   ```sh
   git clone https://https://github.com/AmIARealDeveloper/Tucil3_15223090.git 
   ```
2. Akses terminal dan ketik
   ```sh
   dotnet run
   ```
3. Masukkan input file path
   ```sh
   C:\...
   ```
4. Pilih algoritma yang ingin digunakan. Ketik 1 untuk Greedy Best First Search, 2 untuk Uniform Cost Search, dan 3 untuk A Star.
5. Tunggu hingga program selesai menjalankan algoritma dan menampilkan hasil

### Author
| NIM      | Nama                            |
| -------- | ------------------------------- |
| 15223090 | Ignacio Kevin Alberiann         |
