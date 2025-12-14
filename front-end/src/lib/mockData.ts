import { Movie } from '@/types';

export const mockMovies: Movie[] = [
  {
    id: '1',
    title: 'The Dark Knight',
    description: 'When the menace known as the Joker wreaks havoc and chaos on the people of Gotham, Batman must accept one of the greatest psychological and physical tests of his ability to fight injustice.',
    poster: 'https://image.tmdb.org/t/p/w500/qJ2tW6WMUDux911r6m7haRef0WH.jpg',
    backdrop: 'https://image.tmdb.org/t/p/original/hkBaDkMWbLaf8B1lsWsKX7Ew3Xq.jpg',
    year: 2008,
    duration: 152,
    rating: 9.0,
    genres: ['Action', 'Crime', 'Drama'],
    tags: ['superhero', 'dark', 'thriller'],
    cast: [
      { id: '1', name: 'Christian Bale', character: 'Bruce Wayne / Batman' },
      { id: '2', name: 'Heath Ledger', character: 'Joker' },
      { id: '3', name: 'Aaron Eckhart', character: 'Harvey Dent' }
    ],
    director: 'Christopher Nolan',
    quality: '4K' as const,
    streamUrl: 'https://demo.unified-streaming.com/k8s/features/stable/video/tears-of-steel/tears-of-steel.ism/.m3u8',
    views: 1500000,
    createdAt: '2024-01-15'
  },
  {
    id: '2',
    title: 'Inception',
    description: 'A thief who steals corporate secrets through the use of dream-sharing technology is given the inverse task of planting an idea into the mind of a C.E.O.',
    poster: 'https://image.tmdb.org/t/p/w500/9gk7adHYeDvHkCSEqAvQNLV5Uge.jpg',
    backdrop: 'https://image.tmdb.org/t/p/original/s3TBrRGB1iav7gFOCNx3H31MoES.jpg',
    year: 2010,
    duration: 148,
    rating: 8.8,
    genres: ['Action', 'Sci-Fi', 'Thriller'],
    tags: ['mind-bending', 'dreams', 'heist'],
    cast: [
      { id: '4', name: 'Leonardo DiCaprio', character: 'Dom Cobb' },
      { id: '5', name: 'Joseph Gordon-Levitt', character: 'Arthur' },
      { id: '6', name: 'Ellen Page', character: 'Ariadne' }
    ],
    director: 'Christopher Nolan',
    quality: '4K' as const,
    streamUrl: 'https://demo.unified-streaming.com/k8s/features/stable/video/tears-of-steel/tears-of-steel.ism/.m3u8',
    views: 1200000,
    createdAt: '2024-01-15'
  },
  {
    id: '3',
    title: 'Interstellar',
    description: "A team of explorers travel through a wormhole in space in an attempt to ensure humanity's survival.",
    poster: 'https://image.tmdb.org/t/p/w500/gEU2QniE6E77NI6lCU6MxlNBvIx.jpg',
    backdrop: 'https://image.tmdb.org/t/p/original/xJHokMbljvjADYdit5fK5VQsXEG.jpg',
    year: 2014,
    duration: 169,
    rating: 8.6,
    genres: ['Adventure', 'Drama', 'Sci-Fi'],
    tags: ['space', 'time-travel', 'emotional'],
    cast: [
      { id: '7', name: 'Matthew McConaughey', character: 'Cooper' },
      { id: '8', name: 'Anne Hathaway', character: 'Brand' },
      { id: '9', name: 'Jessica Chastain', character: 'Murph' }
    ],
    director: 'Christopher Nolan',
    quality: '4K' as const,
    streamUrl: 'https://demo.unified-streaming.com/k8s/features/stable/video/tears-of-steel/tears-of-steel.ism/.m3u8',
    views: 980000,
    createdAt: '2024-01-15'
  },
  {
    id: '4',
    title: 'Pulp Fiction',
    description: 'The lives of two mob hitmen, a boxer, a gangster and his wife intertwine in four tales of violence and redemption.',
    poster: 'https://image.tmdb.org/t/p/w500/d5iIlFn5s0ImszYzBPb8JPIfbXD.jpg',
    backdrop: 'https://image.tmdb.org/t/p/original/suaEOtk1N1sgg2MTM7oZd2cfVp3.jpg',
    year: 1994,
    duration: 154,
    rating: 8.9,
    genres: ['Crime', 'Drama'],
    tags: ['classic', 'non-linear', 'cult'],
    cast: [
      { id: '10', name: 'John Travolta', character: 'Vincent Vega' },
      { id: '11', name: 'Uma Thurman', character: 'Mia Wallace' },
      { id: '12', name: 'Samuel L. Jackson', character: 'Jules Winnfield' }
    ],
    director: 'Quentin Tarantino',
    quality: '1080p' as const,
    streamUrl: 'https://demo.unified-streaming.com/k8s/features/stable/video/tears-of-steel/tears-of-steel.ism/.m3u8',
    views: 850000,
    createdAt: '2024-01-15'
  },
  {
    id: '5',
    title: 'The Matrix',
    description: 'A computer hacker learns from mysterious rebels about the true nature of his reality and his role in the war against its controllers.',
    poster: 'https://image.tmdb.org/t/p/w500/f89U3ADr1oiB1s9GkdPOEpXUk5H.jpg',
    backdrop: 'https://image.tmdb.org/t/p/original/fNG7i7RqMErkcqhohV2a6cV1Ehy.jpg',
    year: 1999,
    duration: 136,
    rating: 8.7,
    genres: ['Action', 'Sci-Fi'],
    tags: ['cyberpunk', 'philosophy', 'action'],
    cast: [
      { id: '13', name: 'Keanu Reeves', character: 'Neo' },
      { id: '14', name: 'Laurence Fishburne', character: 'Morpheus' },
      { id: '15', name: 'Carrie-Anne Moss', character: 'Trinity' }
    ],
    director: 'Lana Wachowski',
    quality: '4K' as const,
    streamUrl: 'https://demo.unified-streaming.com/k8s/features/stable/video/tears-of-steel/tears-of-steel.ism/.m3u8',
    views: 1100000,
    createdAt: '2024-01-15'
  },
  {
    id: '6',
    title: 'Fight Club',
    description: 'An insomniac office worker and a devil-may-care soap maker form an underground fight club that evolves into much more.',
    poster: 'https://image.tmdb.org/t/p/w500/pB8BM7pdSp6B6Ih7QZ4DrQ3PmJK.jpg',
    backdrop: 'https://image.tmdb.org/t/p/original/hZkgoQYus5vegHoetLkCJzb17zJ.jpg',
    year: 1999,
    duration: 139,
    rating: 8.8,
    genres: ['Drama'],
    tags: ['psychological', 'twist', 'dark'],
    cast: [
      { id: '16', name: 'Brad Pitt', character: 'Tyler Durden' },
      { id: '17', name: 'Edward Norton', character: 'The Narrator' },
      { id: '18', name: 'Helena Bonham Carter', character: 'Marla Singer' }
    ],
    director: 'David Fincher',
    quality: '1080p' as const,
    streamUrl: 'https://demo.unified-streaming.com/k8s/features/stable/video/tears-of-steel/tears-of-steel.ism/.m3u8',
    views: 920000,
    createdAt: '2024-01-15'
  },
  {
    id: '7',
    title: 'The Shawshank Redemption',
    description: 'Two imprisoned men bond over a number of years, finding solace and eventual redemption through acts of common decency.',
    poster: 'https://image.tmdb.org/t/p/w500/q6y0Go1tsGEsmtFryDOJo3dEmqu.jpg',
    backdrop: 'https://image.tmdb.org/t/p/original/kXfqcdQKsToO0OUXHcrrNCHDBzO.jpg',
    year: 1994,
    duration: 142,
    rating: 9.3,
    genres: ['Drama'],
    tags: ['prison', 'hope', 'friendship'],
    cast: [
      { id: '19', name: 'Tim Robbins', character: 'Andy Dufresne' },
      { id: '20', name: 'Morgan Freeman', character: 'Ellis Boyd Redding' },
      { id: '21', name: 'Bob Gunton', character: 'Warden Norton' }
    ],
    director: 'Frank Darabont',
    quality: '4K' as const,
    streamUrl: 'https://demo.unified-streaming.com/k8s/features/stable/video/tears-of-steel/tears-of-steel.ism/.m3u8',
    views: 2100000,
    createdAt: '2024-01-15'
  },
  {
    id: '8',
    title: 'Forrest Gump',
    description: 'The presidencies of Kennedy and Johnson, the Vietnam War, and other historical events unfold from the perspective of an Alabama man.',
    poster: 'https://image.tmdb.org/t/p/w500/saHP97rTPS5eLmrLQEcANmKrsFl.jpg',
    backdrop: 'https://image.tmdb.org/t/p/original/3h1JZGDhZ8nzxdgvkxha0qBqi05.jpg',
    year: 1994,
    duration: 142,
    rating: 8.8,
    genres: ['Drama', 'Romance'],
    tags: ['inspirational', 'journey', 'heartwarming'],
    cast: [
      { id: '22', name: 'Tom Hanks', character: 'Forrest Gump' },
      { id: '23', name: 'Robin Wright', character: 'Jenny Curran' },
      { id: '24', name: 'Gary Sinise', character: 'Lt. Dan Taylor' }
    ],
    director: 'Robert Zemeckis',
    quality: '4K' as const,
    streamUrl: 'https://demo.unified-streaming.com/k8s/features/stable/video/tears-of-steel/tears-of-steel.ism/.m3u8',
    views: 1850000,
    createdAt: '2024-01-15'
  },
  {
    id: '9',
    title: 'The Godfather',
    description: 'The aging patriarch of an organized crime dynasty transfers control of his clandestine empire to his reluctant son.',
    poster: 'https://image.tmdb.org/t/p/w500/3bhkrj58Vtu7enYsRolD1fZdja1.jpg',
    backdrop: 'https://image.tmdb.org/t/p/original/tmU7GeKVybMWFButWEGl2M4GeiP.jpg',
    year: 1972,
    duration: 175,
    rating: 9.2,
    genres: ['Crime', 'Drama'],
    tags: ['mafia', 'classic', 'family'],
    cast: [
      { id: '25', name: 'Marlon Brando', character: 'Don Vito Corleone' },
      { id: '26', name: 'Al Pacino', character: 'Michael Corleone' },
      { id: '27', name: 'James Caan', character: 'Sonny Corleone' }
    ],
    director: 'Francis Ford Coppola',
    quality: '4K' as const,
    streamUrl: 'https://demo.unified-streaming.com/k8s/features/stable/video/tears-of-steel/tears-of-steel.ism/.m3u8',
    views: 1950000,
    createdAt: '2024-01-15'
  },
  {
    id: '10',
    title: 'Parasite',
    description: 'Greed and class discrimination threaten the newly formed symbiotic relationship between the wealthy Park family and the destitute Kim clan.',
    poster: 'https://image.tmdb.org/t/p/w500/7IiTTgloJzvGI1TAYymCfbfl3vT.jpg',
    backdrop: 'https://image.tmdb.org/t/p/original/TU9NIjwzjoKPwQHoHshkFcQUCG.jpg',
    year: 2019,
    duration: 132,
    rating: 8.6,
    genres: ['Drama', 'Thriller'],
    tags: ['social-commentary', 'korean', 'suspense'],
    cast: [
      { id: '28', name: 'Song Kang-ho', character: 'Kim Ki-taek' },
      { id: '29', name: 'Lee Sun-kyun', character: 'Park Dong-ik' },
      { id: '30', name: 'Cho Yeo-jeong', character: 'Choi Yeon-gyo' }
    ],
    director: 'Bong Joon-ho',
    quality: '4K' as const,
    streamUrl: 'https://demo.unified-streaming.com/k8s/features/stable/video/tears-of-steel/tears-of-steel.ism/.m3u8',
    views: 1350000,
    createdAt: '2024-01-15'
  },
  {
    id: '11',
    title: 'Gladiator',
    description: 'A former Roman General sets out to exact vengeance against the corrupt emperor who murdered his family and sent him into slavery.',
    poster: 'https://image.tmdb.org/t/p/w500/ty8TGRuvJLPUmAR1H1nRIsgwvim.jpg',
    backdrop: 'https://image.tmdb.org/t/p/original/6wkfovpn7Eq8dYNKaG5PY3q2oq6.jpg',
    year: 2000,
    duration: 155,
    rating: 8.5,
    genres: ['Action', 'Drama', 'Adventure'],
    tags: ['epic', 'revenge', 'historical'],
    cast: [
      { id: '31', name: 'Russell Crowe', character: 'Maximus' },
      { id: '32', name: 'Joaquin Phoenix', character: 'Commodus' },
      { id: '33', name: 'Connie Nielsen', character: 'Lucilla' }
    ],
    director: 'Ridley Scott',
    quality: '4K' as const,
    streamUrl: 'https://demo.unified-streaming.com/k8s/features/stable/video/tears-of-steel/tears-of-steel.ism/.m3u8',
    views: 1420000,
    createdAt: '2024-01-15'
  },
  {
    id: '12',
    title: 'The Prestige',
    description: 'After a tragic accident, two stage magicians engage in a battle to create the ultimate illusion while sacrificing everything they have to outwit each other.',
    poster: 'https://image.tmdb.org/t/p/w500/tRNlZbgNCNOpLpbPEz5L8G8A0JN.jpg',
    backdrop: 'https://image.tmdb.org/t/p/original/h2wkoI2EpbCwness6fSpabNDmJl.jpg',
    year: 2006,
    duration: 130,
    rating: 8.5,
    genres: ['Drama', 'Mystery', 'Thriller'],
    tags: ['magic', 'rivalry', 'twist'],
    cast: [
      { id: '34', name: 'Christian Bale', character: 'Alfred Borden' },
      { id: '35', name: 'Hugh Jackman', character: 'Robert Angier' },
      { id: '36', name: 'Scarlett Johansson', character: 'Olivia Wenscombe' }
    ],
    director: 'Christopher Nolan',
    quality: '4K' as const,
    streamUrl: 'https://demo.unified-streaming.com/k8s/features/stable/video/tears-of-steel/tears-of-steel.ism/.m3u8',
    views: 1180000,
    createdAt: '2024-01-15'
  },
  {
    id: '13',
    title: 'Avengers: Endgame',
    description: 'After the devastating events of Infinity War, the Avengers assemble once more to reverse Thanos actions and restore balance to the universe.',
    poster: 'https://image.tmdb.org/t/p/w500/or06FN3Dka5tukK1e9sl16pB3iy.jpg',
    backdrop: 'https://image.tmdb.org/t/p/original/7RyHsO4yDXtBv1zUU3mTpHeQ0d5.jpg',
    year: 2019,
    duration: 181,
    rating: 8.4,
    genres: ['Action', 'Adventure', 'Sci-Fi'],
    tags: ['superhero', 'epic', 'mcu'],
    cast: [
      { id: '37', name: 'Robert Downey Jr.', character: 'Tony Stark / Iron Man' },
      { id: '38', name: 'Chris Evans', character: 'Steve Rogers / Captain America' },
      { id: '39', name: 'Scarlett Johansson', character: 'Natasha Romanoff / Black Widow' }
    ],
    director: 'Anthony Russo, Joe Russo',
    quality: '4K' as const,
    streamUrl: 'https://demo.unified-streaming.com/k8s/features/stable/video/tears-of-steel/tears-of-steel.ism/.m3u8',
    views: 2450000,
    createdAt: '2024-01-15'
  },
  {
    id: '14',
    title: 'Joker',
    description: 'In Gotham City, mentally troubled comedian Arthur Fleck is disregarded and mistreated by society, leading him to embrace his darker side.',
    poster: 'https://image.tmdb.org/t/p/w500/udDclJoHjfjb8Ekgsd4FDteOkCU.jpg',
    backdrop: 'https://image.tmdb.org/t/p/original/n6bUvigpRFqSwmPp1m2YADdbRBc.jpg',
    year: 2019,
    duration: 122,
    rating: 8.4,
    genres: ['Crime', 'Drama', 'Thriller'],
    tags: ['psychological', 'dark', 'origin-story'],
    cast: [
      { id: '40', name: 'Joaquin Phoenix', character: 'Arthur Fleck / Joker' },
      { id: '41', name: 'Robert De Niro', character: 'Murray Franklin' },
      { id: '42', name: 'Zazie Beetz', character: 'Sophie Dumond' }
    ],
    director: 'Todd Phillips',
    quality: '4K' as const,
    streamUrl: 'https://demo.unified-streaming.com/k8s/features/stable/video/tears-of-steel/tears-of-steel.ism/.m3u8',
    views: 1720000,
    createdAt: '2024-01-15'
  },
  {
    id: '15',
    title: 'Spider-Man: No Way Home',
    description: 'With Spider-Man identity now revealed, Peter asks Doctor Strange for help. When a spell goes wrong, dangerous foes from other worlds start to appear.',
    poster: 'https://image.tmdb.org/t/p/w500/1g0dhYtq4irTY1GPXvft6k4YLjm.jpg',
    backdrop: 'https://image.tmdb.org/t/p/original/VlHt27nCqOuTnuX6bku8QZapzO.jpg',
    year: 2021,
    duration: 148,
    rating: 8.3,
    genres: ['Action', 'Adventure', 'Sci-Fi'],
    tags: ['superhero', 'multiverse', 'spider-man'],
    cast: [
      { id: '43', name: 'Tom Holland', character: 'Peter Parker / Spider-Man' },
      { id: '44', name: 'Zendaya', character: 'MJ' },
      { id: '45', name: 'Benedict Cumberbatch', character: 'Doctor Strange' }
    ],
    director: 'Jon Watts',
    quality: '4K' as const,
    streamUrl: 'https://demo.unified-streaming.com/k8s/features/stable/video/tears-of-steel/tears-of-steel.ism/.m3u8',
    views: 2280000,
    createdAt: '2024-01-15'
  },
  {
    id: '16',
    title: 'Oppenheimer',
    description: 'The story of American scientist J. Robert Oppenheimer and his role in the development of the atomic bomb.',
    poster: 'https://image.tmdb.org/t/p/w500/8Gxv8gSFCU0XGDykEGv7zR1n2ua.jpg',
    backdrop: 'https://image.tmdb.org/t/p/original/fm6KqXpk3M2HVveHwCrBSSBaO0V.jpg',
    year: 2023,
    duration: 180,
    rating: 8.5,
    genres: ['Drama', 'History', 'Biography'],
    tags: ['historical', 'war', 'biography'],
    cast: [
      { id: '46', name: 'Cillian Murphy', character: 'J. Robert Oppenheimer' },
      { id: '47', name: 'Emily Blunt', character: 'Kitty Oppenheimer' },
      { id: '48', name: 'Robert Downey Jr.', character: 'Lewis Strauss' }
    ],
    director: 'Christopher Nolan',
    quality: '4K' as const,
    streamUrl: 'https://demo.unified-streaming.com/k8s/features/stable/video/tears-of-steel/tears-of-steel.ism/.m3u8',
    views: 1890000,
    createdAt: '2024-01-15'
  },
  {
    id: '17',
    title: 'Dune',
    description: 'Paul Atreides, a brilliant young man born into a great destiny, must travel to the most dangerous planet to ensure the future of his family and people.',
    poster: 'https://image.tmdb.org/t/p/w500/d5NXSklXo0qyIYkgV94XAgMIckC.jpg',
    backdrop: 'https://image.tmdb.org/t/p/original/s1FhAJTV7JHweiNP6TInezA77ok.jpg',
    year: 2021,
    duration: 155,
    rating: 8.1,
    genres: ['Sci-Fi', 'Adventure'],
    tags: ['epic', 'desert', 'space'],
    cast: [
      { id: '49', name: 'Timothée Chalamet', character: 'Paul Atreides' },
      { id: '50', name: 'Zendaya', character: 'Chani' },
      { id: '51', name: 'Rebecca Ferguson', character: 'Lady Jessica' }
    ],
    director: 'Denis Villeneuve',
    quality: '4K' as const,
    streamUrl: 'https://demo.unified-streaming.com/k8s/features/stable/video/tears-of-steel/tears-of-steel.ism/.m3u8',
    views: 1640000,
    createdAt: '2024-01-15'
  },
  {
    id: '18',
    title: 'The Lord of the Rings: The Fellowship of the Ring',
    description: 'A meek Hobbit and eight companions set out on a journey to destroy a powerful ring and save Middle-earth from the Dark Lord Sauron.',
    poster: 'https://image.tmdb.org/t/p/w500/6oom5QYQ2yQTMJIbnvbkBL9cHo6.jpg',
    backdrop: 'https://image.tmdb.org/t/p/original/pIUvQ9Ed35wlWhY2oU6OmwEsmzG.jpg',
    year: 2001,
    duration: 178,
    rating: 8.8,
    genres: ['Adventure', 'Fantasy'],
    tags: ['epic', 'fantasy', 'quest'],
    cast: [
      { id: '52', name: 'Elijah Wood', character: 'Frodo Baggins' },
      { id: '53', name: 'Ian McKellen', character: 'Gandalf' },
      { id: '54', name: 'Viggo Mortensen', character: 'Aragorn' }
    ],
    director: 'Peter Jackson',
    quality: '4K' as const,
    streamUrl: 'https://demo.unified-streaming.com/k8s/features/stable/video/tears-of-steel/tears-of-steel.ism/.m3u8',
    views: 2050000,
    createdAt: '2024-01-15'
  }
];

export const getMockTrending = (): Movie[] => mockMovies.slice(0, 10);
export const getMockPopular = (): Movie[] => mockMovies.slice(5, 15);
export const getMockTopRated = (): Movie[] => mockMovies.slice(0, 8);
