/**
 * Wisconsin Chess Association — Site Data
 * ========================================
 * Edit this file to update site content without touching HTML/CSS/JS.
 * All content is stored as plain JavaScript objects and arrays.
 */

const WCA_DATA = {

  // ── Site Settings ──────────────────────────────────────────────
  site: {
    name: "Wisconsin Chess Association",
    abbr: "WCA",
    tagline: "Advancing Chess Across the Badger State",
    email: "info@wischess.org",
    phone: "(608) 555-0182",
    address: "Madison, Wisconsin",
    uscfAffiliateId: "WI-001",
    socialLinks: {
      facebook:  "https://facebook.com/wischess",
      twitter:   "https://twitter.com/wischess",
      instagram: "https://instagram.com/wischess",
    }
  },

  // ── Navigation ─────────────────────────────────────────────────
  nav: [
    { label: "Home",       href: "index.html",      id: "home" },
    { label: "Events",     href: "events.html",     id: "events" },
    { label: "WI Tour",    href: "wi-tour.html",    id: "wi-tour" },
    { label: "News",       href: "news.html",       id: "news" },
    { label: "Clubs",      href: "clubs.html",      id: "clubs" },
    { label: "Board",      href: "board.html",      id: "board" },
    { label: "Resources",  href: "resources.html",  id: "resources" },
  ],

  // ── Announcement Banner (set to null to hide) ─────────────────
  announcement: {
    text: "🏆 State Championship Registration Now Open — Early Bird Deadline June 15th.",
    linkText: "Register Now",
    linkHref: "events.html#state-championship"
  },

  // ── Events ────────────────────────────────────────────────────
  // type: "tour" | "club" | "scholastic" | "national" | "meeting"
  events: [
    {
      id: "evt-001",
      title: "WI Tour — Madison Open",
      type: "tour",
      date: "2025-06-07",
      endDate: "2025-06-08",
      location: "Madison Senior Center",
      city: "Madison, WI",
      timeControl: "G/90 d5",
      sections: ["Open", "U1800", "U1400"],
      entryFee: "$35 / $25 juniors",
      tourPoints: 40,
      prizes: "$1,200 based on entries",
      url: "#",
      registerUrl: "#",
      featured: true,
      description: "One of the strongest regular-season stops on the WI Tour, the Madison Open draws players from across the state. Rated by USCF. Free parking. Hotel block available at Courtyard Madison."
    },
    {
      id: "evt-002",
      title: "Green Bay Chess Club Open",
      type: "club",
      date: "2025-06-14",
      location: "Brown County Library",
      city: "Green Bay, WI",
      timeControl: "G/60 d5",
      sections: ["Open"],
      entryFee: "$20",
      tourPoints: 20,
      prizes: "$300 guaranteed",
      url: "#",
      registerUrl: "#",
      featured: false,
      description: "Monthly rated club tournament. All are welcome. Bring your own clock."
    },
    {
      id: "evt-003",
      title: "WI State Scholastic Championship",
      type: "scholastic",
      date: "2025-06-21",
      endDate: "2025-06-22",
      location: "Monona Terrace",
      city: "Madison, WI",
      timeControl: "G/30 d5",
      sections: ["K-12 Open", "K-8 Open", "K-5 Open", "K-3 Open"],
      entryFee: "$25 per player",
      tourPoints: 0,
      prizes: "Trophies & medals",
      url: "#",
      registerUrl: "#",
      featured: true,
      description: "Annual statewide scholastic championship. Students from all Wisconsin schools are invited to compete in age-appropriate sections."
    },
    {
      id: "evt-004",
      title: "WI Tour — Milwaukee City Championship",
      type: "tour",
      date: "2025-07-12",
      endDate: "2025-07-13",
      location: "Milwaukee Public Library",
      city: "Milwaukee, WI",
      timeControl: "G/90 d5",
      sections: ["Open", "U2000", "U1600", "U1200"],
      entryFee: "$40 / $25 juniors / $30 seniors",
      tourPoints: 50,
      prizes: "$1,500 based on entries",
      url: "#",
      registerUrl: "#",
      featured: true,
      description: "The Milwaukee City Championship is a premier WI Tour event with a strong Open section and excellent prize fund. Hotel discounts available."
    },
    {
      id: "evt-005",
      title: "WCA Board Meeting — Q3",
      type: "meeting",
      date: "2025-07-19",
      location: "Zoom (Virtual)",
      city: "Online",
      timeControl: null,
      sections: [],
      entryFee: null,
      tourPoints: 0,
      prizes: null,
      url: "#",
      registerUrl: null,
      featured: false,
      description: "Quarterly board meeting. Agenda posted 7 days prior on the Board page."
    },
    {
      id: "evt-006",
      title: "Appleton Summer Classic",
      type: "club",
      date: "2025-07-26",
      location: "Appleton Public Library",
      city: "Appleton, WI",
      timeControl: "G/45 d5",
      sections: ["Open", "U1600"],
      entryFee: "$20",
      tourPoints: 20,
      prizes: "$400 based on entries",
      url: "#",
      registerUrl: "#",
      featured: false,
      description: "Annual summer event hosted by the Fox Valley Chess Club."
    },
    {
      id: "evt-007",
      title: "WI State Championship",
      type: "tour",
      date: "2025-08-16",
      endDate: "2025-08-17",
      location: "Kalahari Resort",
      city: "Wisconsin Dells, WI",
      timeControl: "G/90 d5",
      sections: ["Championship", "U2000", "U1800", "U1500", "U1200"],
      entryFee: "$55 / $35 juniors",
      tourPoints: 100,
      prizes: "$3,000 guaranteed",
      url: "#",
      registerUrl: "#",
      featured: true,
      description: "The premier event of the Wisconsin chess calendar. The State Champion earns the WCA State Title and automatic qualification to the US Chess National Amateur. Tour points awarded across all sections."
    },
    {
      id: "evt-008",
      title: "Wausau Blitz Tournament",
      type: "club",
      date: "2025-08-23",
      location: "Marathon County Library",
      city: "Wausau, WI",
      timeControl: "G/5 d0 (Blitz)",
      sections: ["Open"],
      entryFee: "$15",
      tourPoints: 10,
      prizes: "$200 based on entries",
      url: "#",
      registerUrl: "#",
      featured: false,
      description: "Fun blitz event, unrated. Great for players of all levels."
    }
  ],

  // ── WI Tour Standings ─────────────────────────────────────────
  tourSeason: {
    year: "2024–25",
    totalEvents: 12,
    completedEvents: 8,
    nextEvent: "evt-001",
    description: "The WI Tour is a statewide competition where players earn points at rated WCA-sanctioned events. The top player in each section at year-end receives the WI Tour Trophy and a cash prize."
  },

  tourStandings: [
    { rank: 1, name: "Marcus Johansson",   club: "Madison Chess Club",      rating: 2241, points: 310, events: 6, wins: 4, trend: "up",   results: [50, 50, 40, 40, 40, 40, 50, 0] },
    { rank: 2, name: "Priya Nair",         club: "Milwaukee Chess Society", rating: 2188, points: 290, events: 7, wins: 3, trend: "up",   results: [50, 40, 40, 40, 40, 40, 40, 0] },
    { rank: 3, name: "Kevin Schraeder",    club: "Fox Valley Chess Club",   rating: 2155, points: 265, events: 6, wins: 3, trend: "same", results: [40, 40, 50, 40, 40, 40, 15, 0] },
    { rank: 4, name: "Amelia Torres",      club: "Madison Chess Club",      rating: 2102, points: 250, events: 8, wins: 2, trend: "up",   results: [40, 40, 40, 40, 20, 40, 30, 0] },
    { rank: 5, name: "Daniel Przybylski",  club: "Green Bay Chess Club",    rating: 2078, points: 235, events: 7, wins: 2, trend: "down", results: [40, 40, 40, 40, 40, 20, 15, 0] },
    { rank: 6, name: "Sarah Holmberg",     club: "Wausau Chess Club",       rating: 1984, points: 210, events: 5, wins: 2, trend: "up",   results: [50, 40, 40, 40, 40, 0, 0, 0] },
    { rank: 7, name: "James Okonkwo",      club: "Racine Chess Club",       rating: 1947, points: 195, events: 6, wins: 1, trend: "same", results: [40, 40, 40, 20, 20, 20, 15, 0] },
    { rank: 8, name: "Ling Wei",           club: "Madison Chess Club",      rating: 1921, points: 185, events: 6, wins: 1, trend: "down", results: [40, 40, 40, 20, 20, 25, 0, 0] },
    { rank: 9, name: "Erik Sundqvist",     club: "Fox Valley Chess Club",   rating: 1898, points: 170, events: 5, wins: 1, trend: "same", results: [40, 40, 40, 25, 25, 0, 0, 0] },
    { rank: 10, name: "Natasha Brennan",   club: "Milwaukee Chess Society", rating: 1876, points: 160, events: 7, wins: 1, trend: "up",   results: [20, 20, 40, 20, 20, 20, 20, 0] }
  ],

  tourEvents: [
    { name: "Racine Winter Open",        date: "2024-11-02", pts: 40, status: "complete" },
    { name: "Madison Holiday Classic",   date: "2024-12-07", pts: 40, status: "complete" },
    { name: "Green Bay Invitational",    date: "2025-01-11", pts: 40, status: "complete" },
    { name: "Fox Valley Open",           date: "2025-02-08", pts: 40, status: "complete" },
    { name: "Milwaukee Winter Open",     date: "2025-03-01", pts: 50, status: "complete" },
    { name: "Wausau Spring Classic",     date: "2025-04-05", pts: 40, status: "complete" },
    { name: "Appleton April Open",       date: "2025-04-26", pts: 40, status: "complete" },
    { name: "Kenosha Open",              date: "2025-05-10", pts: 40, status: "complete" },
    { name: "Madison Open",              date: "2025-06-07", pts: 40, status: "upcoming" },
    { name: "Milwaukee City Championship",date:"2025-07-12", pts: 50, status: "upcoming" },
    { name: "Eau Claire Summer Open",    date: "2025-07-26", pts: 40, status: "upcoming" },
    { name: "WI State Championship",     date: "2025-08-16", pts: 100, status: "upcoming" },
  ],

  // ── Board of Directors ────────────────────────────────────────
  board: [
    {
      name: "Robert Vang",
      role: "President",
      initials: "RV",
      email: "president@wischess.org",
      bio: "Robert has served as President since 2022. A USCF Life Member with a peak rating of 2180, he has been instrumental in growing tournament participation across the state.",
      term: "2023–2025",
      committee: ["Executive", "Tournament"]
    },
    {
      name: "Linda Kasowski",
      role: "Vice President",
      initials: "LK",
      email: "vp@wischess.org",
      bio: "Linda coordinates outreach and scholastic programs. She oversees the annual state scholastic championship and coordinates with school districts statewide.",
      term: "2023–2025",
      committee: ["Executive", "Scholastic"]
    },
    {
      name: "Thomas Brandt",
      role: "Treasurer",
      initials: "TB",
      email: "treasurer@wischess.org",
      bio: "Thomas manages federation finances, grant applications, and membership fee processing. CPA by profession with 15 years in nonprofit financial management.",
      term: "2024–2026",
      committee: ["Executive", "Finance"]
    },
    {
      name: "Maria Chen",
      role: "Secretary",
      initials: "MC",
      email: "secretary@wischess.org",
      bio: "Maria records meeting minutes, maintains official records, and handles membership communications. She also coordinates the WCA newsletter.",
      term: "2024–2026",
      committee: ["Executive", "Communications"]
    },
    {
      name: "Darnell Williams",
      role: "Tournament Director",
      initials: "DW",
      email: "td@wischess.org",
      bio: "Darnell is a USCF Senior TD and oversees sanctioning, WI Tour coordination, and tournament standards across the state.",
      term: "2023–2025",
      committee: ["Tournament"]
    },
    {
      name: "Annika Sorenson",
      role: "Scholastic Coordinator",
      initials: "AS",
      email: "scholastic@wischess.org",
      bio: "Annika runs the WCA's K-12 chess programs and works with coaches, advisors, and school administrators across Wisconsin.",
      term: "2024–2026",
      committee: ["Scholastic"]
    },
    {
      name: "Paul Lemke",
      role: "Webmaster & Communications",
      initials: "PL",
      email: "web@wischess.org",
      bio: "Paul manages the WCA website, social media, and digital communications. He also coordinates with USCF for affiliate reporting.",
      term: "2024–2026",
      committee: ["Communications"]
    },
    {
      name: "Gloria Hutchins",
      role: "At-Large Director",
      initials: "GH",
      email: "board@wischess.org",
      bio: "Gloria represents the southern Wisconsin region and focuses on community outreach and junior player development initiatives.",
      term: "2023–2025",
      committee: ["Outreach"]
    }
  ],

  // ── Recent Board Activity ──────────────────────────────────────
  boardActivity: [
    {
      date: "2025-04-19",
      type: "Meeting Minutes",
      title: "Q2 Board Meeting — April 2025",
      summary: "Approved 2025 State Championship budget, reviewed WI Tour midseason standings, voted to increase junior player entry fee discounts to 40%.",
      url: "#"
    },
    {
      date: "2025-03-10",
      type: "Resolution",
      title: "Resolution 2025-03: WI Tour Tiebreak Rules Update",
      summary: "Adopted updated tiebreak procedures for WI Tour point ties at year-end. Modified Bucholz calculation to include only top-4 scored events.",
      url: "#"
    },
    {
      date: "2025-02-15",
      type: "Financial Report",
      title: "Q1 2025 Financial Summary",
      summary: "Membership revenue up 12% YoY. Tournament surplus of $3,240 from winter events. Scholastic program grant renewal approved by USCF.",
      url: "#"
    },
    {
      date: "2025-01-18",
      type: "Meeting Minutes",
      title: "Q1 Board Meeting — January 2025",
      summary: "Elected officers for 2025, approved event calendar through August, welcomed two new At-Large directors, approved 2025 operating budget.",
      url: "#"
    }
  ],

  // ── News & Articles ───────────────────────────────────────────
  // source: "state" | "national" | "international"
  news: [
    {
      id: "news-001",
      title: "Wisconsin's Priya Nair Wins Midwest Invitational",
      source: "state",
      category: "Tournament Results",
      date: "2025-05-28",
      author: "WCA Staff",
      summary: "Madison native Priya Nair claimed the top prize at the 2025 Midwest Invitational in Chicago, going 6.5/7 and defeating two Grandmasters in the process.",
      url: "#",
      featured: true,
      emoji: "♛"
    },
    {
      id: "news-002",
      title: "US Chess Announces Rule Updates for 2025–26",
      source: "national",
      category: "USCF News",
      date: "2025-05-20",
      author: "US Chess",
      summary: "The USCF Policy Board has approved several rule modifications effective September 2025, including changes to clock handling and touch-move clarifications.",
      url: "#",
      featured: false,
      emoji: "📋"
    },
    {
      id: "news-003",
      title: "WCA Launches Free After-School Chess Initiative",
      source: "state",
      category: "Community",
      date: "2025-05-14",
      author: "Annika Sorenson",
      summary: "The Wisconsin Chess Association is partnering with 18 Milwaukee Unified School District schools to bring free chess instruction to over 1,200 students starting fall 2025.",
      url: "#",
      featured: false,
      emoji: "🏫"
    },
    {
      id: "news-004",
      title: "Magnus Carlsen Announces Return to Classical Chess",
      source: "international",
      category: "World Chess",
      date: "2025-05-10",
      author: "FIDE Press",
      summary: "Former World Champion Magnus Carlsen has confirmed he will enter the 2026 World Championship cycle, setting up a potential rematch with current champion Ding Liren.",
      url: "#",
      featured: false,
      emoji: "🌍"
    },
    {
      id: "news-005",
      title: "Fox Valley Chess Club Celebrates 50 Years",
      source: "state",
      category: "Club News",
      date: "2025-05-05",
      author: "WCA Staff",
      summary: "The Fox Valley Chess Club, based in Appleton, is celebrating its 50th anniversary with a jubilee tournament in July. The club has produced four state champions.",
      url: "#",
      featured: false,
      emoji: "🎉"
    },
    {
      id: "news-006",
      title: "2025 US Open: Wisconsin Players Shine in St. Louis",
      source: "national",
      category: "Tournament Results",
      date: "2025-04-28",
      author: "WCA Staff",
      summary: "Three Wisconsin players finished in prize-winning positions at the 2025 US Open. Kevin Schraeder tied for first in the U2200 section with 7.5 points.",
      url: "#",
      featured: false,
      emoji: "🏅"
    }
  ],

  // ── Local Clubs ───────────────────────────────────────────────
  clubs: [
    {
      name: "Madison Chess Club",
      city: "Madison",
      region: "South",
      website: "#",
      email: "info@madisonchess.org",
      meetingDay: "Tuesdays & Thursdays",
      meetingTime: "6:30 PM",
      location: "Madison Senior Center, 330 W Mifflin St",
      members: 87,
      contactName: "Gary Petersen",
      update: "New Tuesday rapid series starting June 3rd. Beginners welcome — free instruction available 6:00–6:30 PM before club play.",
      lastUpdated: "2025-05-20"
    },
    {
      name: "Milwaukee Chess Society",
      city: "Milwaukee",
      region: "Southeast",
      website: "#",
      email: "info@milwaukeechess.org",
      meetingDay: "Wednesdays",
      meetingTime: "7:00 PM",
      location: "Milwaukee Public Library — Central, 814 W Wisconsin Ave",
      members: 112,
      contactName: "Tanisha Moore",
      update: "Summer tournament schedule posted. June rated event on the 14th. New club champion crowned — congratulations to Priya Nair!",
      lastUpdated: "2025-05-18"
    },
    {
      name: "Fox Valley Chess Club",
      city: "Appleton",
      region: "Northeast",
      website: "#",
      email: "contact@foxvalleychess.org",
      meetingDay: "Mondays",
      meetingTime: "6:00 PM",
      location: "Appleton Public Library, 225 N Oneida St",
      members: 54,
      contactName: "Erik Sundqvist",
      update: "50th Anniversary Jubilee Tournament registration open. Space limited to 60 players. Club shirts available for members — order by June 1st.",
      lastUpdated: "2025-05-15"
    },
    {
      name: "Green Bay Chess Club",
      city: "Green Bay",
      region: "Northeast",
      website: "#",
      email: "greenbay@chess.org",
      meetingDay: "Thursdays",
      meetingTime: "7:00 PM",
      location: "Brown County Library — Central, 515 Pine St",
      members: 41,
      contactName: "Peter Halverson",
      update: "Monthly rated game June 14th at the library. Looking for TD volunteer — contact Peter if interested.",
      lastUpdated: "2025-05-12"
    },
    {
      name: "Wausau Chess Club",
      city: "Wausau",
      region: "Central",
      website: "#",
      email: "wausau@wischess.org",
      meetingDay: "Fridays",
      meetingTime: "6:30 PM",
      location: "Marathon County Library, 300 N First St",
      members: 29,
      contactName: "Sarah Holmberg",
      update: "Blitz tournament August 23rd, open to all. No registration required — show up and play!",
      lastUpdated: "2025-05-08"
    },
    {
      name: "Racine Chess Club",
      city: "Racine",
      region: "Southeast",
      website: "#",
      email: "racine@wischess.org",
      meetingDay: "Tuesdays",
      meetingTime: "7:00 PM",
      location: "Racine Public Library, 75 7th St",
      members: 33,
      contactName: "James Okonkwo",
      update: "Seeking new meeting venue — current library hours restricting late games. Contact James if you can assist. Club still meeting as normal.",
      lastUpdated: "2025-05-01"
    }
  ]

}; // END WCA_DATA
