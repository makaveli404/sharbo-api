namespace SharboAPI.Infrastructure.Seeding;

/// <summary>
/// Demo data for local development: the shared Firebase test accounts of the dev project (sharbo-1)
/// and the groups and entries created for them. Passwords are not here; they live in the git-ignored
/// test-accounts.json that developers pass to each other.
/// </summary>
public static class DevData
{
	public sealed record Account(string Uid, string Nickname, string Email);

	public enum EntryKind { Quote, Situation, Meme }

	/// <param name="Author">Nickname of the group member who added the entry.</param>
	/// <param name="ImageSlug">Memes only: seed for a placeholder image from picsum.photos.</param>
	public sealed record Entry(EntryKind Kind, string Author, string Text, string? ImageSlug = null)
	{
		public string? ImageUrl => ImageSlug is null ? null : $"https://picsum.photos/seed/sharbo-{ImageSlug}/800/600";
	}

	/// <param name="Admin">Nickname of the creator; gets the Admin, Moderator and Participant roles.</param>
	/// <param name="Members">Other members (nicknames); they get the Participant role.</param>
	/// <param name="Moderators">Members who additionally get the Moderator role.</param>
	public sealed record Group(string Name, string Admin, string[] Members, string[] Moderators, Entry[] Entries);

	public static readonly Account[] Accounts =
	[
		new("HY056ztLEPQhNXfp5XdDKUp5MpA2", "Ola", "tester01@example.com"),
		new("451urzOe8RZEH9Aq0TW8OWNWEUB3", "Kuba", "tester02@example.com"),
		new("lDoWOJXiRSepSj202NGsRbu4aT93", "Michał", "tester03@example.com"),
		new("sYk94Bl7stV8LEzRHfEC3xvl1373", "Zosia", "tester04@example.com"),
		new("vw4tkENTjISEsyhHG60cYXOGQCI2", "Bartek", "tester05@example.com"),
		new("GyWmpVWozLawns9jAQyQJt9DONi2", "Kasia", "tester06@example.com"),
		new("0ON9mMAOO1VNyeTvgWc5Ti3bw953", "Tomek", "tester07@example.com"),
		new("5yBp1Ex06hP5guLCRG4oyx1QU7R2", "Ania", "tester08@example.com"),
		new("AryOpsbhHzYyfzqRzNag9NPNU0h2", "Piotrek", "tester09@example.com"),
		new("ST55OsuYf2Y2wy2lsfDNSTojQD13", "Magda", "tester10@example.com")
	];

	public static readonly Group[] Groups =
	[
		new("Ekipa z akademika", Admin: "Ola", Members: ["Kuba", "Michał", "Zosia"], Moderators: ["Kuba"],
		[
			new(EntryKind.Quote, "Kuba", "To nie jest kebab, to jest stan umysłu."),
			new(EntryKind.Quote, "Ola", "Ja tylko na jedno."),
			new(EntryKind.Quote, "Michał", "Kolokwium jest jutro, czyli mam jeszcze całą noc."),
			new(EntryKind.Situation, "Zosia", "Zosia zamówiła taksówkę pod adres, pod którym właśnie stała."),
			new(EntryKind.Situation, "Kuba", "Kuba szukał telefonu, świecąc sobie latarką z tego telefonu."),
			new(EntryKind.Situation, "Ola", "Ola zrobiła pranie białych rzeczy z czerwoną skarpetką Michała."),
			new(EntryKind.Meme, "Michał", "Michał o 7:58 przed kolokwium", "kolokwium"),
			new(EntryKind.Meme, "Zosia", "Projekt grupowy: 1 osoba robi, 3 są na bieżąco", "projekt")
		]),
		new("Bieszczady 2026", Admin: "Bartek", Members: ["Kasia", "Tomek", "Ola"], Moderators: [],
		[
			new(EntryKind.Quote, "Tomek", "To tylko 4 kilometry, płasko jak stół."),
			new(EntryKind.Quote, "Kasia", "Ktoś wziął zapalniczkę? No właśnie."),
			new(EntryKind.Quote, "Bartek", "Mapa jest dla słabych, idziemy na czuja."),
			new(EntryKind.Situation, "Bartek", "Bartek rozbił namiot na mrowisku i obwinił mrówki."),
			new(EntryKind.Situation, "Tomek", "Tomek wszedł na Tarnicę w klapkach i był pierwszy."),
			new(EntryKind.Meme, "Kasia", "Prognoza: słońce. Bieszczady: nie.", "prognoza"),
			new(EntryKind.Meme, "Ola", "Ostatnie zdjęcie przed sześcioma godzinami deszczu", "deszcz")
		]),
		new("Ola i Kuba", Admin: "Ola", Members: ["Kuba"], Moderators: [],
		[
			new(EntryKind.Quote, "Kuba", "Nie jestem głodny. Zjem tylko trochę twoich frytek."),
			new(EntryKind.Quote, "Ola", "Wybierz film. Nie ten. Ten też nie."),
			new(EntryKind.Situation, "Kuba", "Kuba przez 20 minut szukał okularów, które miał na głowie."),
			new(EntryKind.Meme, "Ola", "Kuba, kiedy słyszy otwieranie lodówki", "lodowka")
		]),
		new("Open space, 3 piętro", Admin: "Ania", Members: ["Piotrek", "Magda", "Tomek", "Michał"], Moderators: ["Piotrek"],
		[
			new(EntryKind.Quote, "Piotrek", "U mnie działa."),
			new(EntryKind.Quote, "Magda", "To spotkanie mogło być mailem."),
			new(EntryKind.Quote, "Michał", "Wrzucę to na produkcję w piątek o 16:55. Co może pójść źle?"),
			new(EntryKind.Quote, "Ania", "Kto podgrzał rybę w mikrofali, ten wie, co zrobił."),
			new(EntryKind.Situation, "Tomek", "Tomek był na callu z włączoną kamerą, myśląc, że ją wyłączył."),
			new(EntryKind.Situation, "Magda", "Magda przez pół godziny mówiła na wyciszonym mikrofonie."),
			new(EntryKind.Meme, "Piotrek", "Deploy w piątek: oczekiwania kontra rzeczywistość", "deploy"),
			new(EntryKind.Meme, "Ania", "Ekspres do kawy zepsuty. Produktywność: 0%", "ekspres")
		]),
		new("FIFA w każdy czwartek", Admin: "Piotrek", Members: ["Magda", "Kasia", "Zosia", "Bartek"], Moderators: [],
		[
			new(EntryKind.Quote, "Bartek", "Pad mi się zaciął, to się nie liczy."),
			new(EntryKind.Quote, "Zosia", "Gram pierwszy raz, który przycisk strzela?"),
			new(EntryKind.Quote, "Magda", "Rewanż. Do trzech wygranych. Albo do pięciu."),
			new(EntryKind.Situation, "Zosia", "Zosia wygrała turniej, grając pierwszy raz w życiu."),
			new(EntryKind.Situation, "Piotrek", "Piotrek wyłączył konsolę przy 0:5, bo „coś laguje”."),
			new(EntryKind.Meme, "Kasia", "Bartek po przegranej: to wina pada", "pad")
		])
	];
}
